using JRiver;
using System;
using System.Threading;

namespace MCMonitor
{
    // Monitors MC state, [re]connects when it [re]starts
    // Receives MC events and fires OnMCEvent to trigger MCEventQueue
    internal static class MCMonitor
    {
        static Thread monitor;
        static int counter = 0;
        public static JAutomation api { get; private set; } = new JAutomation();

        public static event EventHandler<bool> OnConnectionChanged;
        public static event EventHandler<MCEventInfo> OnMCEvent;

        public static void Start()
        {
            // start MC monitor thread
            monitor = new Thread(MonitorMC);
            monitor.IsBackground = true;
            monitor.Name = "MCMonitorThread";
            monitor.Start();
        }

        public static void Stop()
        {
            monitor?.Interrupt();
            monitor?.Join(500);
            monitor = null;
        }

        // check connection periodically and re-connect if needed
        private static void MonitorMC()
        {
            bool starting = true;
            while (true)
            {
                try
                {
                    bool connected = api.Connected;
                    api.Connect();

                    if (starting || api.Connected != connected)
                        ConnectionChanged(api.Connected);

                    starting = false;
                    Thread.Sleep(1000);
                }
                catch (ThreadInterruptedException) { break; }
                catch (ThreadAbortException) { break; }
                catch (Exception ex) { Logger.Log(ex); }
            }
        }

        private static void ConnectionChanged(bool connected)
        {
            Logger.Log($"MC is {(connected ? "connected" : "disconnected")}");

            OnConnectionChanged?.Invoke(api, connected);

            if (connected)
            {
                api.MCEvent += McAPI_MCEvent;
                ReportEvent(EventSource.MCMonitor, new JAutomationEvent("MCMonitor", "CONNECTED", $"version:{api.MCVersion};Library:{api.Library}"));
            }
            else if (api != null)
            {
                api.MCEvent -= McAPI_MCEvent;
                ReportEvent(EventSource.MCMonitor, new JAutomationEvent("MCMonitor", "DISCONNECTED", null));
            }
        }

        private static void McAPI_MCEvent(object sender, JAutomationEvent e)
        {
            ReportEvent(EventSource.MCEvent, e);
        }

        private static void ReportEvent(EventSource type, JAutomationEvent e)
        {
            if (OnMCEvent == null) return;
            MCEventInfo info = new MCEventInfo()
            {
                Arg1 = e.param1?.Replace("MCC: ", ""),
                Arg2 = e.param2,
                Type = e.type?.Replace("MJEvent type: ", ""),
                Timestamp = DateTime.Now,
                EventCounter = Interlocked.Increment(ref counter),
                Source = EventSource.MCEvent
            };

            try { OnMCEvent?.Invoke(api, info); } catch { }
        }
    }
}