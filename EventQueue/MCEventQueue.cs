using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace MCMonitor
{
    // Receives MC Events sent from MCMonitor
    // Queues events into a FIFO queue then fires OnEventQueued to trigger MCEventProcessor
    internal class MCEventQueue
    {
        // event handlers
        JsonHandler jsonWriter;
        LogHandler logWriter;
        WebSocketHandler websocket;
        ExecuteHandler executer;

        ConcurrentQueue<MCEventInfo> eventQueue = new ConcurrentQueue<MCEventInfo>();
        AutoResetEvent signal = new AutoResetEvent(false);
        Thread queueThread;
        volatile bool stopping = false;
        int counter = -1;

        string eventFilter;
        string[] fieldList;
        string fieldQuery;

        public static int ProcessedEvents = 0;
        public static int FilteredEvents = 0;
        public static int ProcessingErrors = 0;

        bool FetchPlaybackInfo;
        bool FetchFileInfo;
        SafeDictionary<string, string> PlaybackInfo;
        SafeDictionary<string, string> CurrentFileInfo;
        SafeDictionary<string, string> NextFileInfo;
        DateTime InfoDate;

        public MCEventQueue(Config config)
        {
            eventFilter = config[ConfigProperty.Filter];
            fieldList = Regex.Split(config[ConfigProperty.Fields] ?? "", @"\,(?![^[]*\])").Select(f => f.Trim().Trim('[').Trim(']')).ToArray();
            if (fieldList != null && fieldList.Length > 0)
                fieldQuery = $"&Fields={string.Join(",", fieldList.Select(f => Uri.EscapeDataString(f)))}";

            jsonWriter = new JsonHandler(config);
            logWriter = new LogHandler(config);
            websocket = new WebSocketHandler(config);
            executer = new ExecuteHandler(config);

            FetchFileInfo = jsonWriter.NeedsDetails || logWriter.NeedsDetails || websocket.NeedsDetails || executer.NeedsDetails;
            FetchPlaybackInfo = FetchFileInfo || jsonWriter.NeedsPlaybackInfo || logWriter.NeedsPlaybackInfo || websocket.NeedsPlaybackInfo || executer.NeedsPlaybackInfo;
        }

        public void Start()
        {
            OnMCEventReceived(null, new MCEventInfo() { Source = EventSource.MCMonitor, Timestamp = DateTime.Now, Type = "STARTED" });

            // start event thread
            stopping = false;
            queueThread = new Thread(ProcessQueueThread);
            queueThread.IsBackground = true;
            queueThread.Name = "MCEventQueueThread";
            queueThread.Start();

            // register MC monitor
            MCMonitor.OnMCEvent += OnMCEventReceived;
        }

        public void Stop()
        {
            MCMonitor.OnMCEvent -= OnMCEventReceived;

            OnMCEventReceived(null, new MCEventInfo() { Source = EventSource.MCMonitor, Timestamp = DateTime.Now, Type = "EXITING" });

            // stop command executer
            executer.Stop();
            websocket.Stop();

            // flush queue and stop handlers
            stopping = true;
            signal.Set();
            if (!queueThread.Join(1000))
                queueThread.Interrupt();

            queueThread = null;
        }

        void OnMCEventReceived(object sender, MCEventInfo e)
        {
            e.EventCounter = Interlocked.Increment(ref counter);
            eventQueue.Enqueue(e);
            signal.Set();
        }

        void ProcessQueueThread()
        {
            while (!stopping)
            {
                try
                {
                    if (signal.WaitOne())
                    {
                        while (eventQueue.TryDequeue(out var eventInfo))
                            ProcessEvent(eventInfo);
                    }
                }
                catch (ThreadAbortException) { break; }
                catch (ThreadInterruptedException) { break; }
            }
        }

        private void ProcessEvent(MCEventInfo e)
        {
            try
            {
                // update info
                if (UpdatePlaybackInfo())
                    InfoDate = e.Timestamp;

                // always log regardless of filter
                Process(logWriter, e);

                // check filter
                if (isFilteredOut(e))
                {
                    FilteredEvents++;
                    Logger.Log($"Event {e.EventCounter} filtered out: {e}");
                }
                else
                {
                    // process event
                    ProcessedEvents++;
                    Process(jsonWriter, e);
                    Process(websocket, e);
                    Process(executer, e);
                }
            }
            catch (Exception ex) 
            {
                ProcessingErrors++;
                Logger.Log(ex, $"Exception processing event {e}"); 
            }
        }

        private bool isFilteredOut(MCEventInfo e)
        {
            // apply filter
            if (string.IsNullOrWhiteSpace(eventFilter))
                return false;

            try
            {
                return !Regex.IsMatch(e.ToString(), eventFilter, RegexOptions.IgnoreCase);
            }
            catch { }
            return true;    // regex failed
        }

        private void Process(BaseHandler writer, MCEventInfo e)
        {
            if (!writer.isEnabled)
                return;

            // set or clear current info for this writer
            e.PlaybackInfo = (writer.NeedsPlaybackInfo || writer.NeedsDetails) ? PlaybackInfo : null;
            e.CurrentFile = writer.NeedsDetails ? CurrentFileInfo : null;
            e.NextFile = writer.NeedsDetails ? NextFileInfo : null;
            e.InfoTimestamp = e.PlaybackInfo != null ? InfoDate : null;

            writer.Process(e);
        }

        private bool UpdatePlaybackInfo()
        {
            bool updated = false;
            if (FetchPlaybackInfo)
            {
                var info = GetPlaybackInfo();
                if (info != null)
                {
                    PlaybackInfo = info;
                    updated = true;
                    
                }
            }

            if (FetchFileInfo)
            {
                var curr = GetFile(PlaybackInfo?["FileKey"], fieldList) ?? CurrentFileInfo;
                var next = GetFile(PlaybackInfo?["NextFileKey"], fieldList) ?? NextFileInfo;
                if (curr != null) CurrentFileInfo = curr;
                if (next != null) NextFileInfo = next;
            }

            return updated;
        }

        private SafeDictionary<string, string> GetPlaybackInfo()
        {
            string xml = MCMonitor.api.WebRequest("/MCWS/v1/Playback/Info");
            if (string.IsNullOrEmpty(xml))
                return null;

            var comparer = StringComparer.OrdinalIgnoreCase;
            var dict = new SafeDictionary<string, string>(comparer);
            
            var props = Regex.Matches(xml, @"<Item Name=""(.+?)"">(.+)</Item>", RegexOptions.Multiline);
            foreach (Match prop in props)
                dict[prop.Groups[1].Value] = prop.Groups[2].Value;

            return dict;
        }

        private SafeDictionary<string, string> GetFile(string key, string[] fieldList)
        {
           if (string.IsNullOrEmpty(key))
                return null;

            try
            {
                string json = MCMonitor.api.WebRequest($"/MCWS/v1/File/Getinfo?File={key}&Action=json{fieldQuery}");
                if (string.IsNullOrEmpty(json))
                    return null;

                var comparer = StringComparer.OrdinalIgnoreCase;
                var dict = new SafeDictionary<string, string>(comparer);
                
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (data != null && data.Count > 0)
                    foreach (var pair in data[0])
                        dict[pair.Key] = pair.Value.ToString();

                return dict;
            }
            catch { }
            return null;
        }
    }
}
