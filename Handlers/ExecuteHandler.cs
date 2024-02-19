using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace MCMonitor
{
    public class Command
    {
        public string command;
        public MCEventInfo info;

        public Command(string cmd, MCEventInfo e)
        {
            command = cmd;
            info = e;
        }
    }

    internal class ExecuteHandler : BaseHandler
    {
        double minWait = 0;
        double maxWait = 1;

        ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();
        AutoResetEvent signal = new AutoResetEvent(false);
        Thread queueThread;
        volatile bool stopping = false;
        DateTime lastExecution = DateTime.Now;

        public ExecuteHandler(Config config) : base(config)
        {
            isEnabled = config.ExecuteList != null && config.ExecuteList.Count > 0;
            if (!isEnabled) return;

            if (!double.TryParse(config[ConfigProperty.ExecuteMinWait], out minWait) || minWait < 0) minWait = 0;
            if (!double.TryParse(config[ConfigProperty.ExecuteMinWait], out maxWait) || maxWait < 0) maxWait = 1;

            foreach (var cmd in config.ExecuteList)
            {
                if (Regex.IsMatch(cmd, @"\$(state|currkey|nextkey)|\$i\[", RegexOptions.IgnoreCase)) NeedsPlaybackInfo = true;
                if (Regex.IsMatch(cmd, @"\$\$?\[")) NeedsDetails = true;
            }

            Start();
        }

        public override bool Process(MCEventInfo e)
        {
            if (!isEnabled) return false;

            foreach (var cmd in config.ExecuteList)
                commandQueue.Enqueue(new Command(cmd, e));

            signal.Set();
            return true;
        }

        public void Start()
        {
            // start event thread
            stopping = false;
            queueThread = new Thread(CommandQueueThread);
            queueThread.IsBackground = true;
            queueThread.Name = "CommandQueueThread";
            queueThread.Start();
        }

        public void Stop()
        {
            if (!isEnabled) return;

            // flush queue and stop handlers
            stopping = true;
            signal.Set();
            try
            {
                if (!queueThread.Join(2000))
                    queueThread.Interrupt();
            }
            catch { }

            queueThread = null;
        }


        void CommandQueueThread()
        {
            while (!stopping)
            {
                try
                {
                    if (signal.WaitOne())
                    {
                        while (commandQueue.TryDequeue(out var command))
                            ProcessCommand(command);
                    }
                }
                catch (ThreadAbortException) { break; }
                catch (ThreadInterruptedException) { break; }
            }
        }

        void ProcessCommand(Command command)
        {
            // minWait
            var now = DateTime.Now;
            var next = lastExecution.AddSeconds(minWait);
            if (minWait > 0 && now < next)
                Thread.Sleep(next - now);

            lastExecution = DateTime.Now;
            string cmd = FillTemplate(command.command, command.info);
            executeShellCommand(cmd, (int)(maxWait * 1000));
        }

        private static bool executeShellCommand(string command, int timeout = 0, string workingDirectory = null)
        {
            var args = command.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
            if (command.StartsWith("\""))
                args = command.TrimStart('"').Split("\"", 2, StringSplitOptions.RemoveEmptyEntries);

            bool isSuccess = false;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = args[0];
                startInfo.Arguments = args[1];
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WorkingDirectory = workingDirectory ?? "";

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    Logger.Log($"Running command: {startInfo.FileName} {startInfo.Arguments}");

                    if (!process.Start())
                    {
                        Logger.Log("Failed to start process");
                        return false;
                    }

                    if (timeout <= 0)
                        return true;

                    isSuccess = process.WaitForExit(timeout);
                    if (!isSuccess)
                        Logger.Log("Process did not exit in the given timeout ({timeout} ms)");
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, $"Exception running command");
            }

            return isSuccess;
        }

    }
}
