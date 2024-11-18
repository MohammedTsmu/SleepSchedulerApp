using System;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class LockScreenForm : Form
    {
        public LockScreenForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_TOPMOST
                return cp;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Disable Alt+F4
            if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Optionally, add a timer to close the form after sleep time ends
    }
}
