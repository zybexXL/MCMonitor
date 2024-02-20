using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MCMonitor
{
#pragma warning disable CA1416 // Validate platform compatibility
    public partial class TrayForm : Form
    {
        Config config;
        bool showBallon = true;
        bool isVisible = false;     // the Form.Visible flag is not reliable

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        public TrayForm(Config config)
        {
            InitializeComponent();

            this.Icon = trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.config = config;
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            lblVersion.Text = $"MCMonitor v{ver} by Zybex";

            // allow drag by clicking anywhere
            foreach (Control c in Controls)
                if (c is Label)
                    c.MouseDown += TrayForm_MouseDown;

            // register for MC events
            MCMonitor.OnMCEvent += (s, e) => UpdateCounters();

            // start in the corner and hide
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
            if (!isVisible)
                SetVisible(isVisible);

            UpdateCounters();
        }

        private void TrayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
