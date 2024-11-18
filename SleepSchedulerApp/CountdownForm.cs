using System;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class CountdownForm : Form
    {
        private int countdownTime;
        private Timer countdownTimer;

        public CountdownForm(int seconds, bool cancellable = true)
        {
            InitializeComponent();
            this.TopMost = true;

            // Optional UI settings
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            countdownTime = seconds;

            // Use the 'cancellable' parameter to control the Cancel button
            buttonCancel.Enabled = cancellable;
            buttonCancel.Visible = cancellable;

            // Initialize label with the countdown message
            UpdateCountdownLabel();

            countdownTimer = new Timer
            {
                Interval = 1000
            };
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTime--;
            UpdateCountdownLabel();

            if (countdownTime <= 0)
            {
                countdownTimer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void UpdateCountdownLabel()
        {
            labelCountdown.Text = $"The computer will shut down in {countdownTime} seconds. Please save your work immediately.";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            countdownTimer.Dispose();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
