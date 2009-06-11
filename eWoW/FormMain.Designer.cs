namespace eWoW
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.chkAutoRestartWorld = new System.Windows.Forms.CheckBox();
            this.chkAutoRestartLogon = new System.Windows.Forms.CheckBox();
            this.btnStartServers = new System.Windows.Forms.Button();
            this.ctxMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ntfyMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.tmrCheckProcs = new System.Windows.Forms.Timer(this.components);
            this.chkHideWindows = new System.Windows.Forms.CheckBox();
            this.ctxMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkAutoRestartWorld
            // 
            this.chkAutoRestartWorld.AutoSize = true;
            this.chkAutoRestartWorld.Location = new System.Drawing.Point(12, 12);
            this.chkAutoRestartWorld.Name = "chkAutoRestartWorld";
            this.chkAutoRestartWorld.Size = new System.Drawing.Size(140, 17);
            this.chkAutoRestartWorld.TabIndex = 0;
            this.chkAutoRestartWorld.Text = "Auto restart world server";
            this.chkAutoRestartWorld.UseVisualStyleBackColor = true;
            // 
            // chkAutoRestartLogon
            // 
            this.chkAutoRestartLogon.AutoSize = true;
            this.chkAutoRestartLogon.Location = new System.Drawing.Point(12, 35);
            this.chkAutoRestartLogon.Name = "chkAutoRestartLogon";
            this.chkAutoRestartLogon.Size = new System.Drawing.Size(141, 17);
            this.chkAutoRestartLogon.TabIndex = 1;
            this.chkAutoRestartLogon.Text = "Auto restart logon server";
            this.chkAutoRestartLogon.UseVisualStyleBackColor = true;
            // 
            // btnStartServers
            // 
            this.btnStartServers.Location = new System.Drawing.Point(12, 86);
            this.btnStartServers.Name = "btnStartServers";
            this.btnStartServers.Size = new System.Drawing.Size(75, 23);
            this.btnStartServers.TabIndex = 2;
            this.btnStartServers.Text = "Start Server";
            this.btnStartServers.UseVisualStyleBackColor = true;
            this.btnStartServers.Click += new System.EventHandler(this.btnStartServers_Click);
            // 
            // ctxMain
            // 
            this.ctxMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.ctxMain.Name = "ctxMain";
            this.ctxMain.Size = new System.Drawing.Size(104, 48);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ntfyMain
            // 
            this.ntfyMain.ContextMenuStrip = this.ctxMain;
            this.ntfyMain.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfyMain.Icon")));
            this.ntfyMain.Text = "eWoW Server";
            this.ntfyMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ntfyMain_MouseDoubleClick);
            // 
            // tmrCheckProcs
            // 
            this.tmrCheckProcs.Interval = 1000;
            this.tmrCheckProcs.Tick += new System.EventHandler(this.tmrCheckProcs_Tick);
            // 
            // chkHideWindows
            // 
            this.chkHideWindows.AutoSize = true;
            this.chkHideWindows.Location = new System.Drawing.Point(12, 58);
            this.chkHideWindows.Name = "chkHideWindows";
            this.chkHideWindows.Size = new System.Drawing.Size(129, 17);
            this.chkHideWindows.TabIndex = 3;
            this.chkHideWindows.Text = "Hide Server Windows";
            this.chkHideWindows.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(163, 121);
            this.Controls.Add(this.chkHideWindows);
            this.Controls.Add(this.chkAutoRestartLogon);
            this.Controls.Add(this.btnStartServers);
            this.Controls.Add(this.chkAutoRestartWorld);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "eWoW";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.ctxMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoRestartWorld;
        private System.Windows.Forms.CheckBox chkAutoRestartLogon;
        private System.Windows.Forms.Button btnStartServers;
        private System.Windows.Forms.ContextMenuStrip ctxMain;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon ntfyMain;
        private System.Windows.Forms.Timer tmrCheckProcs;
        private System.Windows.Forms.CheckBox chkHideWindows;
    }
}

