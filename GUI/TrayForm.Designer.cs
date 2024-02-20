namespace MCMonitor
{
    partial class TrayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayForm));
            trayIcon = new System.Windows.Forms.NotifyIcon(components);
            btnEditConfig = new System.Windows.Forms.Button();
            btnExit = new System.Windows.Forms.Button();
            btnRestart = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            lblVersion = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            btnClose = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            lblReceived = new System.Windows.Forms.Label();
            lblProcessed = new System.Windows.Forms.Label();
            lblIgnored = new System.Windows.Forms.Label();
            lblErrors = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // trayIcon
            // 
            trayIcon.Text = "MC Monitor";
            trayIcon.Visible = true;
            trayIcon.BalloonTipClicked += trayIcon_BalloonTipClicked;
            trayIcon.Click += trayIcon_Click;
            // 
            // btnEditConfig
            // 
            btnEditConfig.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnEditConfig.BackColor = System.Drawing.Color.Gray;
            btnEditConfig.FlatAppearance.BorderSize = 0;
            btnEditConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            btnEditConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEditConfig.ForeColor = System.Drawing.Color.Black;
            btnEditConfig.Location = new System.Drawing.Point(226, 47);
            btnEditConfig.Name = "btnEditConfig";
            btnEditConfig.Size = new System.Drawing.Size(118, 26);
            btnEditConfig.TabIndex = 0;
            btnEditConfig.TabStop = false;
            btnEditConfig.Text = "Edit config file";
            toolTip1.SetToolTip(btnEditConfig, "Edit config.ini file. You'll need to restart to apply any changes.");
            btnEditConfig.UseVisualStyleBackColor = false;
            btnEditConfig.Click += btnEditConfig_Click;
            // 
            // btnExit
            // 
            btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExit.BackColor = System.Drawing.Color.Gray;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExit.ForeColor = System.Drawing.Color.Black;
            btnExit.Location = new System.Drawing.Point(226, 130);
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(118, 26);
            btnExit.TabIndex = 1;
            btnExit.TabStop = false;
            btnExit.Text = "Exit";
            toolTip1.SetToolTip(btnExit, "Exit MCMonitor");
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // btnRestart
            // 
            btnRestart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRestart.BackColor = System.Drawing.Color.Gray;
            btnRestart.FlatAppearance.BorderSize = 0;
            btnRestart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRestart.ForeColor = System.Drawing.Color.Black;
            btnRestart.Location = new System.Drawing.Point(226, 88);
            btnRestart.Name = "btnRestart";
            btnRestart.Size = new System.Drawing.Size(118, 26);
            btnRestart.TabIndex = 1;
            btnRestart.TabStop = false;
            btnRestart.Text = "Restart";
            toolTip1.SetToolTip(btnRestart, "Restart MCMonitor to apply config changes");
            btnRestart.UseVisualStyleBackColor = false;
            btnRestart.Click += btnRestart_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.Color.LightGray;
            label1.Location = new System.Drawing.Point(14, 79);
            label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(112, 17);
            label1.TabIndex = 2;
            label1.Text = "Events Processed:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.LightGray;
            label2.Location = new System.Drawing.Point(28, 106);
            label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(98, 17);
            label2.TabIndex = 2;
            label2.Text = "Events Ignored:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.Color.LightGray;
            label3.Location = new System.Drawing.Point(79, 130);
            label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 17);
            label3.TabIndex = 2;
            label3.Text = "Errors:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.Color.LightGray;
            label4.Location = new System.Drawing.Point(22, 52);
            label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 17);
            label4.TabIndex = 2;
            label4.Text = "Events Received:";
            // 
            // lblVersion
            // 
            lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            lblVersion.AutoSize = true;
            lblVersion.ForeColor = System.Drawing.Color.Gray;
            lblVersion.Location = new System.Drawing.Point(12, 168);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(158, 17);
            lblVersion.TabIndex = 2;
            lblVersion.Text = "MCMonitor v1.0 by Zybex";
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.Color.SteelBlue;
            label6.Location = new System.Drawing.Point(12, 9);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(175, 25);
            label6.TabIndex = 2;
            label6.Text = "MC Event Monitor";
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnClose.BackColor = System.Drawing.Color.Transparent;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(192, 64, 0);
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.Image = (System.Drawing.Image)resources.GetObject("btnClose.Image");
            btnClose.Location = new System.Drawing.Point(334, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(18, 18);
            btnClose.TabIndex = 3;
            btnClose.TabStop = false;
            toolTip1.SetToolTip(btnClose, "Minimize to Tray");
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // lblReceived
            // 
            lblReceived.AutoSize = true;
            lblReceived.ForeColor = System.Drawing.Color.LightGray;
            lblReceived.Location = new System.Drawing.Point(132, 52);
            lblReceived.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            lblReceived.Name = "lblReceived";
            lblReceived.Size = new System.Drawing.Size(15, 17);
            lblReceived.TabIndex = 2;
            lblReceived.Text = "0";
            // 
            // lblProcessed
            // 
            lblProcessed.AutoSize = true;
            lblProcessed.ForeColor = System.Drawing.Color.LightGray;
            lblProcessed.Location = new System.Drawing.Point(132, 79);
            lblProcessed.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            lblProcessed.Name = "lblProcessed";
            lblProcessed.Size = new System.Drawing.Size(15, 17);
            lblProcessed.TabIndex = 2;
            lblProcessed.Text = "0";
            // 
            // lblIgnored
            // 
            lblIgnored.AutoSize = true;
            lblIgnored.ForeColor = System.Drawing.Color.LightGray;
            lblIgnored.Location = new System.Drawing.Point(132, 106);
            lblIgnored.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            lblIgnored.Name = "lblIgnored";
            lblIgnored.Size = new System.Drawing.Size(15, 17);
            lblIgnored.TabIndex = 2;
            lblIgnored.Text = "0";
            // 
            // lblErrors
            // 
            lblErrors.AutoSize = true;
            lblErrors.ForeColor = System.Drawing.Color.LightGray;
            lblErrors.Location = new System.Drawing.Point(132, 130);
            lblErrors.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            lblErrors.Name = "lblErrors";
            lblErrors.Size = new System.Drawing.Size(15, 17);
            lblErrors.TabIndex = 2;
            lblErrors.Text = "0";
            // 
            // TrayForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            ClientSize = new System.Drawing.Size(356, 194);
            ControlBox = false;
            Controls.Add(btnClose);
            Controls.Add(label6);
            Controls.Add(lblVersion);
            Controls.Add(label3);
            Controls.Add(lblErrors);
            Controls.Add(lblIgnored);
            Controls.Add(lblProcessed);
            Controls.Add(lblReceived);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnRestart);
            Controls.Add(btnExit);
            Controls.Add(btnEditConfig);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TrayForm";
            ShowInTaskbar = false;
            TopMost = true;
            Shown += TrayForm_Shown;
            KeyPress += TrayForm_KeyPress;
            MouseDown += TrayForm_MouseDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button btnEditConfig;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblReceived;
        private System.Windows.Forms.Label lblProcessed;
        private System.Windows.Forms.Label lblIgnored;
        private System.Windows.Forms.Label lblErrors;
    }
}

