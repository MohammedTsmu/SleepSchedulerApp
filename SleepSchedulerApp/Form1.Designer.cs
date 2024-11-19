namespace SleepSchedulerApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.CheckBox checkBoxStartup;
        private System.Windows.Forms.NumericUpDown numericUpDownRestrictionPeriod;
        private System.Windows.Forms.Label labelRestrictionPeriod;
        private System.Windows.Forms.Label labelLastChangeTime;
        private System.Windows.Forms.Label labelRestrictionInfo;
        private System.Windows.Forms.ToolTip toolTip1;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Initialize controls
            this.labelStart = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.labelEnd = new System.Windows.Forms.Label();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.checkBoxStartup = new System.Windows.Forms.CheckBox();
            this.numericUpDownRestrictionPeriod = new System.Windows.Forms.NumericUpDown();
            this.labelRestrictionPeriod = new System.Windows.Forms.Label();
            this.labelLastChangeTime = new System.Windows.Forms.Label();
            this.labelRestrictionInfo = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRestrictionPeriod)).BeginInit();
            this.SuspendLayout();

            // 
            // labelStart
            // 
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new System.Drawing.Point(12, 9);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(69, 16);
            this.labelStart.TabIndex = 0;
            this.labelStart.Text = "وقت بدء النوم";

            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerStart.Location = new System.Drawing.Point(15, 28);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.ShowUpDown = true;
            this.dateTimePickerStart.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerStart.TabIndex = 1;
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.DateTimePickerStart_ValueChanged);

            // 
            // labelEnd
            // 
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(12, 77);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(82, 16);
            this.labelEnd.TabIndex = 2;
            this.labelEnd.Text = "وقت انتهاء النوم";

            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(15, 96);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.ShowUpDown = true;
            this.dateTimePickerEnd.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerEnd.TabIndex = 3;
            this.dateTimePickerEnd.ValueChanged += new System.EventHandler(this.DateTimePickerEnd_ValueChanged);

            // 
            // labelRestrictionPeriod
            // 
            this.labelRestrictionPeriod.AutoSize = true;
            this.labelRestrictionPeriod.Location = new System.Drawing.Point(12, 141);
            this.labelRestrictionPeriod.Name = "labelRestrictionPeriod";
            this.labelRestrictionPeriod.Size = new System.Drawing.Size(147, 16);
            this.labelRestrictionPeriod.TabIndex = 4;
            this.labelRestrictionPeriod.Text = "فترة القيود (بالساعات):";

            // 
            // numericUpDownRestrictionPeriod
            // 
            this.numericUpDownRestrictionPeriod.Location = new System.Drawing.Point(15, 160);
            this.numericUpDownRestrictionPeriod.Minimum = new decimal(new int[] {
                0,
                0,
                0,
                0});
            this.numericUpDownRestrictionPeriod.Maximum = new decimal(new int[] {
                24,
                0,
                0,
                0});
            this.numericUpDownRestrictionPeriod.Name = "numericUpDownRestrictionPeriod";
            this.numericUpDownRestrictionPeriod.Size = new System.Drawing.Size(200, 22);
            this.numericUpDownRestrictionPeriod.TabIndex = 5;
            this.numericUpDownRestrictionPeriod.Value = new decimal(new int[] {
                0,
                0,
                0,
                0});
            this.toolTip1.SetToolTip(this.numericUpDownRestrictionPeriod, "مدة الوقت التي لا يمكنك فيها تغيير إعدادات النوم بعد حفظها.");

            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Location = new System.Drawing.Point(15, 200);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(175, 23);
            this.buttonSaveSettings.TabIndex = 6;
            this.buttonSaveSettings.Text = "حفظ الإعدادات";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);

            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.Location = new System.Drawing.Point(15, 230);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new System.Drawing.Size(192, 20);
            this.checkBoxStartup.TabIndex = 7;
            this.checkBoxStartup.Text = "تشغيل التطبيق مع بدء تشغيل النظام";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            this.checkBoxStartup.CheckedChanged += new System.EventHandler(this.checkBoxStartup_CheckedChanged);

            // 
            // labelLastChangeTime
            // 
            this.labelLastChangeTime.AutoSize = true;
            this.labelLastChangeTime.Location = new System.Drawing.Point(15, 260);
            this.labelLastChangeTime.Name = "labelLastChangeTime";
            this.labelLastChangeTime.Size = new System.Drawing.Size(176, 16);
            this.labelLastChangeTime.TabIndex = 8;
            this.labelLastChangeTime.Text = "لم يتم تعديل الإعدادات بعد.";

            // 
            // labelRestrictionInfo
            // 
            this.labelRestrictionInfo.AutoSize = true;
            this.labelRestrictionInfo.Location = new System.Drawing.Point(15, 290);
            this.labelRestrictionInfo.Name = "labelRestrictionInfo";
            this.labelRestrictionInfo.Size = new System.Drawing.Size(400, 16);
            this.labelRestrictionInfo.TabIndex = 9;
            this.labelRestrictionInfo.Text = "بعد حفظ الإعدادات، لن تتمكن من تغييرها مرة أخرى حتى تنتهي فترة القيود المحددة.";

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 330);
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.dateTimePickerStart);
            this.Controls.Add(this.labelEnd);
            this.Controls.Add(this.dateTimePickerEnd);
            this.Controls.Add(this.labelRestrictionPeriod);
            this.Controls.Add(this.numericUpDownRestrictionPeriod);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.checkBoxStartup);
            this.Controls.Add(this.labelLastChangeTime);
            this.Controls.Add(this.labelRestrictionInfo);
            this.Name = "Form1";
            this.Text = "جدولة النوم";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRestrictionPeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
