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

        private DateTime LastNonActivityDateTime;

        public MainWindow()
        {
            InitializeComponent();

            // Custom init
            this.KeyPress += MainWindow_KeyPress;
            this.Resize += MainWindow_Resize;
            this.FormClosing += MainWindow_FormClosing;

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            notifyContextMenuStrip.ItemClicked += NotifyContextMenuStrip_ItemClicked;

            timer.Tick += Timer_Tick;
            timer.Start();
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            double t = SystemIdleTime;
            System.Diagnostics.Debug.Write(t);
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
            Hide();
            notifyIcon.Visible = true;
        }

        private void RestoreFromTray()
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

    }
}
