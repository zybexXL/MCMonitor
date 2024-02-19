using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatsonWebsocket;

namespace MCMonitor
{
    internal static class Program
    {
        internal static string ExecutableFolder => Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName("MCMonitor").Length > 1)
            {
                MessageBox.Show("MCMonitor is already running, please check the Tray icons",
                    "Already running", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // load config
            Config config = Config.Load();
            if (config == null)
            {
                MessageBox.Show($"Failed to load/create config file, please check:\n{Config.ConfigPath}",
                    "Config error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (config.isDefaultConfig)
            {
                MessageBox.Show($"A default config file was created. Click OK to open it in notepad. Please adjust to your needs and restart the application.\n\nConfig file location:\n{Config.ConfigPath}", 
                    "Default config created", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Process.Start("notepad", Config.ConfigPath);
                return;
            }

            Logger.Enabled = config.Debug;
            Logger.Log("Application started");

            // start event handler
            var queue = new MCEventQueue(config);

            // connect to MC and start receiving events
            queue.Start();
            MCMonitor.Start();

            // start tray icon, wait for exit
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form trayForm = new TrayForm(config);
            Application.Run(trayForm);

            // stop and cleanup
            MCMonitor.Stop();
            queue.Stop();
            FileTracker.Purge();

            // restart if needed
            if (trayForm.DialogResult == DialogResult.Retry)
            {
                Logger.Log("Application restarting");
                Application.Restart();
            }
            else
                Logger.Log("Application exiting");
        }
    }
}
