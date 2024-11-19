using System;
using System.Drawing;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class CountdownForm : Form
    {
        private int countdownTime; // Remaining countdown time in seconds
        private Timer countdownTimer; // Timer to handle countdown

        public CountdownForm(int seconds)
        {
            InitializeComponent();
            this.TopMost = true;

            // Set form properties
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            countdownTime = seconds;

            // Initialize label with the countdown message
            UpdateCountdownLabel();

            // Initialize and start the countdown timer
            countdownTimer = new Timer
            {
                Interval = 1000 // 1 second interval
            };
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTime--; // Decrease countdown time by 1 second
            UpdateCountdownLabel();

            if (countdownTime <= 0) // When countdown reaches 0
            {
                countdownTimer.Stop(); // Stop the timer
                this.DialogResult = DialogResult.OK; // Close the form with an OK result
                this.Close();
            }
        }

        private void UpdateCountdownLabel()
        {
            int minutes = countdownTime / 60; // Calculate minutes
            int seconds = countdownTime % 60; // Calculate seconds

            // Update label with the countdown and message
            labelCountdown.Text = $"The computer will shut down in {minutes:D2}:{seconds:D2}. Please save your work.";
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLabelFontSize(); // Adjust font size dynamically when form is resized
        }

        private void AdjustLabelFontSize()
        {
            if (labelCountdown == null || string.IsNullOrEmpty(labelCountdown.Text)) return;

            using (Graphics g = this.CreateGraphics())
            {
                // Measure text size and adjust font size to fit the form
                SizeF textSize = g.MeasureString(labelCountdown.Text, labelCountdown.Font);
                float widthScale = this.ClientSize.Width / textSize.Width;
                float heightScale = this.ClientSize.Height / textSize.Height;
                float scaleFactor = Math.Min(widthScale, heightScale);

                // Apply scaled font size
                float newFontSize = labelCountdown.Font.Size * scaleFactor * 0.6f; // Add buffer to prevent overflow
                labelCountdown.Font = new Font(labelCountdown.Font.FontFamily, Math.Max(newFontSize, 10)); // Ensure minimum font size of 10
            }
        }
    }
}
