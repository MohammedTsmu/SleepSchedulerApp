using System;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class CountdownForm : Form
    {
        private int countdownTime;
        private Timer countdownTimer;

        // Modify the constructor to accept the 'cancellable' parameter
        public CountdownForm(int seconds, bool cancellable = true)
        {
            InitializeComponent();
            countdownTime = seconds;

            // Use the 'cancellable' parameter to control the Cancel button
            buttonCancel.Enabled = cancellable;
            buttonCancel.Visible = cancellable;

            countdownTimer = new Timer
            {
                Interval = 1000
            };
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            UpdateLabel();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTime--;
            UpdateLabel();

            if (countdownTime <= 0)
            {
                countdownTimer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void UpdateLabel()
        {
            labelCountdown.Text = $"Your computer will shut down in {countdownTime} seconds.";
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
