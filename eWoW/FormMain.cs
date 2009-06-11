using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace eWoW
{
    public partial class FormMain : Form
    {
        private const string LogonServerName = "eWoW.LogonServer.exe";
        private const string WorldServerName = "eWoW.WorldServer.exe";
        private Process _logonServerProc;
        private Process _worldServerProc;
        private bool logonServerRunning;
        private bool worldServerRunning;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {}

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                ntfyMain.Visible = true;
            }
            else
            {
                ShowInTaskbar = true;
                ntfyMain.Visible = false;
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ntfyMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void btnStartServers_Click(object sender, EventArgs e)
        {
            StartProc(ref _logonServerProc, LogonServerName);
            StartProc(ref _worldServerProc, WorldServerName);
            tmrCheckProcs.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrCheckProcs.Stop();
            if (_logonServerProc != null && !_logonServerProc.HasExited)
            {
                _logonServerProc.Kill();
            }
            if (_worldServerProc != null && !_worldServerProc.HasExited)
            {
                _worldServerProc.Kill();
            }
        }

        private void tmrCheckProcs_Tick(object sender, EventArgs e)
        {
            worldServerRunning = !_worldServerProc.HasExited;
            logonServerRunning = !_logonServerProc.HasExited;
            if (chkAutoRestartLogon.Checked)
            {
                StartProc(ref _logonServerProc, LogonServerName);
            }
            if (chkAutoRestartWorld.Checked)
            {
                StartProc(ref _worldServerProc, WorldServerName);
            }

            ntfyMain.Text = string.Format("eWoW Server{0}World Server: {1}{0}Logon Server: {2}", "\n",
                                          (worldServerRunning ? "Running" : "Not running"),
                                          (logonServerRunning ? "Running" : "Not running"));
        }

        private void StartProc(ref Process stored, string name)
        {
            if (stored != null && !stored.HasExited)
            {
                return;
            }
            stored = Process.Start(GetStartInfo(name));
        }

        private ProcessStartInfo GetStartInfo(string exeName)
        {
            var psi = new ProcessStartInfo(Application.StartupPath + "\\" + exeName);
            // HIDE THE WINDER!
            if (chkHideWindows.Checked)
            {
                psi.WindowStyle = ProcessWindowStyle.Hidden;
            }

            return psi;
        }
    }
}