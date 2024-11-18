using System.Windows.Forms;

namespace SleepSchedulerApp
{
    partial class CountdownForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelCountdown;
        private Button buttonCancel;

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
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // labelCountdown
            // 
            this.labelCountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.labelCountdown.Location = new System.Drawing.Point(50, 50);
            this.labelCountdown.Name = "labelCountdown";
            this.labelCountdown.Size = new System.Drawing.Size(300, 50);
            this.labelCountdown.Text = "العد التنازلي";
            this.labelCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(150, 120);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.Text = "إلغاء";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // CountdownForm
            // 
            this.ClientSize = new System.Drawing.Size(400, 200);
            this.Controls.Add(this.labelCountdown);
            this.Controls.Add(this.buttonCancel);
            this.Name = "CountdownForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "تنبيه القفل";
            this.ResumeLayout(false);
        }
    }
}
