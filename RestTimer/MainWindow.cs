using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace RestTimer
{
    public partial class MainWindow : Form
    {
        private DateTime dateTime;

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

        private void Timer_Tick(object sender, EventArgs e)
        {
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
