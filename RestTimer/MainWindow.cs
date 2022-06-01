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
        public MainWindow()
        {
            InitializeComponent();
            // Custom init
            this.KeyPress += MainWindow_KeyPress;
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
            throw new NotImplementedException();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Thanks!");
        //}
    }
}
