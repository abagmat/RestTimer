using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RestTimer
{
    public partial class MainWindow : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        private static readonly int kMaxWorkSeconds = 25 * 60;
        private static readonly int kMinRestSeconds = 5 * 60;
        private static readonly int kWorkToleranceSeconds = 10;
        private static readonly int kRestToleranceSeconds = 3;
        private static readonly int kBalloonMilliSeconds = 500;
        private static readonly string kBalloonMessage = "Time to rest.\nClick on tip reset.\n{0:D2}:{1:D2}";
        private static readonly string kBalloonText = "Time to work {0:D2}:{1:D2}";
        private bool WorkMode = true;
        private int TickCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Custom init
            this.KeyPress += MainWindow_KeyPress;
            this.Resize += MainWindow_Resize;
            this.FormClosing += MainWindow_FormClosing;

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            notifyIcon.BalloonTipClosed += NotifyIcon_BalloonTipClosed;
            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
            notifyContextMenuStrip.ItemClicked += NotifyContextMenuStrip_ItemClicked;

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            TickCount = 0;
            WorkMode = true;
        }

        private void NotifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (!WorkMode)
            {
                ShowBalloon();
            }
        }

        public double SystemIdleTime
        {
            get
            {
                int systemUptime = Environment.TickCount;
                int idleTicks = 0;
                LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
                lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
                lastInputInfo.dwTime = 0;
                if (GetLastInputInfo(ref lastInputInfo))
                {
                    int lastInputTicks = (int)lastInputInfo.dwTime;
                    idleTicks = systemUptime - lastInputTicks;
                }
                return ((idleTicks > 0) ? (idleTicks / 1000) : idleTicks);
            }
        }

        public string BalloonMessage
        {
            get
            {
                int countdownMins = (kMinRestSeconds - TickCount) / 60;
                int countdownSecs = (kMinRestSeconds - TickCount) % 60;
                return String.Format(kBalloonMessage, countdownMins, countdownSecs);
            }
        }

        public string BalloonText
        {
            get
            {
                if (WorkMode)
                {
                    int countdownMins = (kMaxWorkSeconds - TickCount) / 60;
                    int countdownSecs = (kMaxWorkSeconds - TickCount) % 60;
                    return String.Format(kBalloonText, countdownMins, countdownSecs);
                }
                return this.Text;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double idleTime = SystemIdleTime;
            if (WorkMode)
            {
                if (idleTime < kWorkToleranceSeconds)
                {
                    TickCount++;
                    if (TickCount > kMaxWorkSeconds)
                    {
                        WorkMode = false;
                        TickCount = 0;
                        ShowBalloon();
                        return;
                    }
                }
                if (idleTime > kMinRestSeconds)
                {
                    TickCount = 0;
                }
                notifyIcon.Text = BalloonText;
                System.Diagnostics.Debug.WriteLine("Work " + TickCount);
                return;
            }
            // Rest time
            if (idleTime > kRestToleranceSeconds)
            {
                TickCount++;
                if (TickCount > kMinRestSeconds)
                {
                    WorkMode = true;
                    TickCount = 0;
                    return;
                }
            }
            notifyIcon.BalloonTipText = BalloonMessage;
            notifyIcon.Text = BalloonText;
            System.Diagnostics.Debug.WriteLine("Rest " + TickCount);
        }

        private void ShowBalloon()
        {
            notifyIcon.ShowBalloonTip(kBalloonMilliSeconds, BalloonText, BalloonMessage, ToolTipIcon.Warning);
        }

        private void NotifyContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag.Equals("tagQuit"))
            {
                notifyIcon.Visible = false;
                Application.Exit();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MinimizeToTray();
                e.Cancel = true;
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreFromTray();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)27:
                    e.Handled = true;
                    MinimizeToTray();
                    break;
            }
        }

        private void MinimizeToTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void RestoreFromTray()
        {
            //this.WindowState = FormWindowState.Normal;
            //this.ShowInTaskbar = true;
        }

    }
}
