using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MCMonitor
{
#pragma warning disable CA1416 // Validate platform compatibility
    public partial class TrayForm : Form
    {
        Config config;
        bool showBallon = true;
        bool isVisible = false;     // the Form.Visible flag is not reliable

        public TrayForm(Config config)
        {
            InitializeComponent();

            this.Icon = trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.config = config;

            MCMonitor.OnMCEvent += (s, e) => UpdateCounters();

            StartPosition = FormStartPosition.Manual;
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width - 2, Screen.PrimaryScreen.WorkingArea.Height - Height - 2);
            SetVisible(false);
        }

        private void UpdateCounters()
        {
            if (this.InvokeRequired)
                BeginInvoke(UpdateCounters);
            else
            {
                lblErrors.Text = MCEventQueue.ProcessingErrors.ToString();
                lblIgnored.Text = MCEventQueue.FilteredEvents.ToString();
                lblProcessed.Text = MCEventQueue.ProcessedEvents.ToString();
                lblReceived.Text = (MCEventQueue.ProcessedEvents + MCEventQueue.FilteredEvents).ToString();
            }
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            SetVisible(!isVisible);
        }

        private void SetVisible(bool visible)
        {
            isVisible = visible;
            this.Visible = visible;
            this.WindowState = visible ? FormWindowState.Normal : FormWindowState.Minimized;

            if (!visible && config.ShowNotifications && showBallon)
            {
                trayIcon.ShowBalloonTip(3000, "", "MCMonitor running in tray", ToolTipIcon.Info);
                showBallon = false;
            }

            if (visible)
            {
                this.BringToFront();
                this.Focus();
            }   
        }

        private void btnEditConfig_Click(object sender, EventArgs e)
        {
            Process.Start("notepad", Path.Combine(Program.ExecutableFolder, "MCMonitor.ini"));
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SetVisible(false);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TrayForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                e.Handled = true;
                SetVisible(false);
            }
        }

        private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            SetVisible(true);
        }

        private void TrayForm_Shown(object sender, EventArgs e)
        {
            UpdateCounters();
        }
    }
}
