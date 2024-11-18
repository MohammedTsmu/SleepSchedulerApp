using System;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class CountdownForm : Form
    {
        private int countdownTime;
        private Timer countdownTimer;

        public CountdownForm(int seconds)
        {
            InitializeComponent();
            countdownTime = seconds;

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
            labelCountdown.Text = $"Your computer will lock in {countdownTime} seconds.";
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
