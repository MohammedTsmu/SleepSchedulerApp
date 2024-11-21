using System.Drawing;

namespace SleepSchedulerApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private System.Windows.Forms.GroupBox groupBoxSleepTime;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;

        private System.Windows.Forms.GroupBox groupBoxRestriction;
        private System.Windows.Forms.Label labelRestrictionPeriod;
        private System.Windows.Forms.NumericUpDown numericUpDownRestrictionPeriod;
        private System.Windows.Forms.Label labelRestrictionInfo;

        private System.Windows.Forms.GroupBox groupBoxAppSettings;
        private System.Windows.Forms.CheckBox checkBoxStartup;

        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.Label labelLastChangeTime;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ToolTip toolTip1;

        private System.Windows.Forms.MenuStrip menuStrip1;

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
            this.groupBoxSleepTime = new System.Windows.Forms.GroupBox();
            this.labelStart = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.labelEnd = new System.Windows.Forms.Label();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();

            this.groupBoxRestriction = new System.Windows.Forms.GroupBox();
            this.labelRestrictionPeriod = new System.Windows.Forms.Label();
            this.numericUpDownRestrictionPeriod = new System.Windows.Forms.NumericUpDown();
            this.labelRestrictionInfo = new System.Windows.Forms.Label();

            this.groupBoxAppSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxStartup = new System.Windows.Forms.CheckBox();

            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.labelLastChangeTime = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);

            this.menuStrip1 = new System.Windows.Forms.MenuStrip();

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRestrictionPeriod)).BeginInit();

            // Form properties
            this.SuspendLayout();
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.ClientSize = new System.Drawing.Size(520, 360);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Icon = Properties.Resources.sleep; // Use 'sleep.ico' for the form's icon
            this.Text = "تطبيق جدولة النوم";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Initialize menuStrip1
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menuStrip1.Size = new System.Drawing.Size(520, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";

            // Create "ملف" (File) menu
            var fileMenuItem = new System.Windows.Forms.ToolStripMenuItem("ملف");
            // Add items to "ملف" menu if needed
            // For example, add an Exit menu item
            var exitMenuItem = new System.Windows.Forms.ToolStripMenuItem("خروج");
            exitMenuItem.Click += (s, args) => this.Close();
            fileMenuItem.DropDownItems.Add(exitMenuItem);

            // Create "مساعدة" (Help) menu
            var helpMenuItem = new System.Windows.Forms.ToolStripMenuItem("مساعدة");
            var aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem("حول");
            aboutMenuItem.Click += (s, args) => ShowAboutDialog();
            helpMenuItem.DropDownItems.Add(aboutMenuItem);

            // Add menus to menuStrip1
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                fileMenuItem,
                helpMenuItem
            });

            // Add menuStrip1 to the form
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;

            // Initialize groupBoxSleepTime
            this.groupBoxSleepTime.Text = "إعدادات وقت النوم";
            this.groupBoxSleepTime.Location = new System.Drawing.Point(15, 35);
            this.groupBoxSleepTime.Size = new System.Drawing.Size(400, 100);
            this.groupBoxSleepTime.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // Initialize labelStart
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new System.Drawing.Point(10, 25);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(69, 16);
            this.labelStart.TabIndex = 0;
            this.labelStart.Text = "وقت بدء النوم";

            // Initialize dateTimePickerStart
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerStart.Location = new System.Drawing.Point(150, 20);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.ShowUpDown = true;
            this.dateTimePickerStart.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerStart.TabIndex = 1;
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.DateTimePickerStart_ValueChanged);

            // Initialize labelEnd
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(10, 60);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(82, 16);
            this.labelEnd.TabIndex = 2;
            this.labelEnd.Text = "وقت انتهاء النوم";

            // Initialize dateTimePickerEnd
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(150, 55);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.ShowUpDown = true;
            this.dateTimePickerEnd.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerEnd.TabIndex = 3;
            this.dateTimePickerEnd.ValueChanged += new System.EventHandler(this.DateTimePickerEnd_ValueChanged);

            // Add controls to groupBoxSleepTime
            this.groupBoxSleepTime.Controls.Add(this.labelStart);
            this.groupBoxSleepTime.Controls.Add(this.dateTimePickerStart);
            this.groupBoxSleepTime.Controls.Add(this.labelEnd);
            this.groupBoxSleepTime.Controls.Add(this.dateTimePickerEnd);

            // Initialize groupBoxRestriction
            this.groupBoxRestriction.Text = "إعدادات القيود";
            this.groupBoxRestriction.Location = new System.Drawing.Point(15, 140);
            this.groupBoxRestriction.Size = new System.Drawing.Size(400, 100);
            this.groupBoxRestriction.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // Initialize labelRestrictionPeriod
            this.labelRestrictionPeriod.AutoSize = true;
            this.labelRestrictionPeriod.Location = new System.Drawing.Point(10, 25);
            this.labelRestrictionPeriod.Name = "labelRestrictionPeriod";
            this.labelRestrictionPeriod.Size = new System.Drawing.Size(147, 16);
            this.labelRestrictionPeriod.TabIndex = 4;
            this.labelRestrictionPeriod.Text = "فترة القيود (بالساعات):";

            // Initialize numericUpDownRestrictionPeriod
            this.numericUpDownRestrictionPeriod.Location = new System.Drawing.Point(150, 20);
            this.numericUpDownRestrictionPeriod.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownRestrictionPeriod.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.numericUpDownRestrictionPeriod.Name = "numericUpDownRestrictionPeriod";
            this.numericUpDownRestrictionPeriod.Size = new System.Drawing.Size(200, 22);
            this.numericUpDownRestrictionPeriod.TabIndex = 5;
            this.toolTip1.SetToolTip(this.numericUpDownRestrictionPeriod, "مدة القيود بالساعات. لن تتمكن من تغيير الإعدادات حتى تنتهي المدة المحددة.");

            // Initialize labelRestrictionInfo
            this.labelRestrictionInfo.AutoSize = true;
            this.labelRestrictionInfo.Location = new System.Drawing.Point(10, 60);
            this.labelRestrictionInfo.Name = "labelRestrictionInfo";
            this.labelRestrictionInfo.Size = new System.Drawing.Size(380, 32);
            this.labelRestrictionInfo.TabIndex = 6;
            this.labelRestrictionInfo.Text = "بعد حفظ الإعدادات، لن تتمكن من تغييرها مرة أخرى حتى تنتهي فترة القيود المحددة.";
            this.labelRestrictionInfo.Visible = false;

            // Add controls to groupBoxRestriction
            this.groupBoxRestriction.Controls.Add(this.labelRestrictionPeriod);
            this.groupBoxRestriction.Controls.Add(this.numericUpDownRestrictionPeriod);
            this.groupBoxRestriction.Controls.Add(this.labelRestrictionInfo);

            // Initialize groupBoxAppSettings
            this.groupBoxAppSettings.Text = "إعدادات التطبيق";
            this.groupBoxAppSettings.Location = new System.Drawing.Point(15, 245);
            this.groupBoxAppSettings.Size = new System.Drawing.Size(400, 60);
            this.groupBoxAppSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // Initialize checkBoxStartup
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.Location = new System.Drawing.Point(10, 25);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new System.Drawing.Size(192, 20);
            this.checkBoxStartup.TabIndex = 7;
            this.checkBoxStartup.Text = "تشغيل التطبيق مع بدء تشغيل النظام";
            this.checkBoxStartup.UseVisualStyleBackColor = true;

            // Add checkBoxStartup to groupBoxAppSettings
            this.groupBoxAppSettings.Controls.Add(this.checkBoxStartup);

            // Initialize buttonSaveSettings
            this.buttonSaveSettings.Location = new System.Drawing.Point(15, 310);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(175, 35);
            this.buttonSaveSettings.TabIndex = 8;
            this.buttonSaveSettings.Text = "حفظ الإعدادات";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);

            // Set button image alignment properties
            this.buttonSaveSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSaveSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSaveSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;

            // Initialize labelLastChangeTime
            this.labelLastChangeTime.AutoSize = true;
            this.labelLastChangeTime.Location = new System.Drawing.Point(200, 317);
            this.labelLastChangeTime.Name = "labelLastChangeTime";
            this.labelLastChangeTime.Size = new System.Drawing.Size(176, 16);
            this.labelLastChangeTime.TabIndex = 9;
            this.labelLastChangeTime.Text = "لم يتم تعديل الإعدادات بعد.";

            // Initialize labelStatus
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(15, 345);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 16);
            this.labelStatus.TabIndex = 10;

            // Add controls to the form
            this.Controls.Add(this.groupBoxSleepTime);
            this.Controls.Add(this.groupBoxRestriction);
            this.Controls.Add(this.groupBoxAppSettings);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.labelLastChangeTime);
            this.Controls.Add(this.labelStatus);

            // Event handlers
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRestrictionPeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}