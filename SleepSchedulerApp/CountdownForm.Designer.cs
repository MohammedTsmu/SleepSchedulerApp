using System;
using System.Drawing;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    partial class CountdownForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelCountdown;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelCountdown = new Label();
            this.SuspendLayout();

            // 
            // labelCountdown
            // 
            this.labelCountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.labelCountdown.Location = new System.Drawing.Point(20, 00);
            this.labelCountdown.Name = "labelCountdown";
            this.labelCountdown.Size = new System.Drawing.Size(360, 100); // Adjust to fit larger text
            this.labelCountdown.Text = "It's almost time to rest.";
            this.labelCountdown.TextAlign = ContentAlignment.MiddleCenter;
            this.labelCountdown.AutoSize = false;

            // 
            // CountdownForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 100);
            this.Controls.Add(this.labelCountdown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CountdownForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "تنبيه القفل";
            this.TopMost = true;

            // Dynamically adjust font size when resizing
            this.Resize += new EventHandler(this.CountdownForm_Resize);
            this.ResumeLayout(false);
        }

        private void CountdownForm_Resize(object sender, EventArgs e)
        {
            AdjustFontSize();
        }

        private void AdjustFontSize()
        {
            if (labelCountdown == null || string.IsNullOrEmpty(labelCountdown.Text)) return;

            using (Graphics g = this.CreateGraphics())
            {
                // Calculate the size of the text and adjust the font size accordingly
                SizeF textSize = g.MeasureString(labelCountdown.Text, labelCountdown.Font);
                float widthScale = this.ClientSize.Width / textSize.Width;
                float heightScale = this.ClientSize.Height / textSize.Height;
                float scaleFactor = Math.Min(widthScale, heightScale);

                float newFontSize = labelCountdown.Font.Size * scaleFactor * 0.8f; // Add a buffer to prevent overflow
                labelCountdown.Font = new Font(labelCountdown.Font.FontFamily, Math.Max(newFontSize, 10)); // Minimum font size of 10
            }
        }
    }
}
