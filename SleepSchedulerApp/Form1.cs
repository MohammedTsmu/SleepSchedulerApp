using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class Form1 : Form
    {
        private DateTime selectedStartTime;
        private DateTime selectedEndTime;

        // Declare timers at class level
        private Timer preSleepTimer;
        //private Timer shutdownTimer;

        private Timer restrictionCheckTimer;
        private Timer statusClearTimer;

        public Form1()
        {
            InitializeComponent();

            // Resize the icon and assign it to the button
            Icon saveIcon = Properties.Resources.save;
            int desiredSize = 24; // Adjust to your preferred size
            Bitmap saveBitmap = new Bitmap(saveIcon.ToBitmap(), new Size(desiredSize, desiredSize));
            this.buttonSaveSettings.Image = saveBitmap;
            this.buttonSaveSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSaveSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSaveSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;


            // Retrieve saved settings
            selectedStartTime = Properties.Settings.Default.StartTime;
            selectedEndTime = Properties.Settings.Default.EndTime;

            dateTimePickerStart.Value = selectedStartTime;
            dateTimePickerEnd.Value = selectedEndTime;
            checkBoxStartup.Checked = Properties.Settings.Default.RunOnStartup;

            // Load the restriction period
            numericUpDownRestrictionPeriod.Value = (decimal)Properties.Settings.Default.RestrictionPeriod;

            // Display the last settings change time
            DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;
            if (lastChange > DateTime.MinValue)
            {
                labelLastChangeTime.Text = $"تم تعديل الإعدادات آخر مرة في: {lastChange}";
            }
            else
            {
                labelLastChangeTime.Text = "لم يتم تعديل الإعدادات بعد.";
            }

            // Subscribe to the SessionSwitch event
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            // Initialize the restrictionCheckTimer but do not start it yet
            restrictionCheckTimer = new Timer();
            restrictionCheckTimer.Interval = 60 * 1000; // 1 minute
            restrictionCheckTimer.Tick += (s, args) =>
            {
                DisableControlsIfRestrictionActive();
            };

            // Initialize the statusClearTimer
            statusClearTimer = new Timer();
            statusClearTimer.Interval = 5000; // 5 seconds
            statusClearTimer.Tick += (s, args) =>
            {
                labelStatus.Text = "";
                statusClearTimer.Stop();
            };

            // Check if the restriction period is active and update controls accordingly
            DisableControlsIfRestrictionActive();

            // Handle FormClosing to prevent closing during sleep time
            this.FormClosing += Form1_FormClosing;

            // Subscribe to the FormClosed event
            this.FormClosed += Form1_FormClosed;

            CheckSleepTime();
        }

        private void DateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            selectedStartTime = dateTimePickerStart.Value;
        }

        private void DateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            selectedEndTime = dateTimePickerEnd.Value;
        }

        private void checkBoxStartup_CheckedChanged(object sender, EventArgs e)
        {
            // Update RunOnStartup setting when saving settings
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            // Get the new sleep times from the DateTimePicker controls
            selectedStartTime = dateTimePickerStart.Value;
            selectedEndTime = dateTimePickerEnd.Value;

            // Adjust validation to handle overnight sleep times
            if (selectedEndTime <= selectedStartTime && selectedEndTime.TimeOfDay <= selectedStartTime.TimeOfDay)
            {
                MessageBox.Show("يجب أن يكون وقت انتهاء النوم بعد وقت بدء النوم.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Check if the restriction period has passed
            DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;
            int savedRestrictionHours = Properties.Settings.Default.RestrictionPeriod;

            if (savedRestrictionHours > 0 && lastChange > DateTime.MinValue)
            {
                DateTime restrictionEndTime = lastChange.AddHours(savedRestrictionHours);

                if (DateTime.Now < restrictionEndTime)
                {
                    TimeSpan timeLeft = restrictionEndTime - DateTime.Now;
                    MessageBox.Show($"لا يمكنك تغيير إعدادات النوم حتى {restrictionEndTime}.\n" +
                                    $"الوقت المتبقي: {timeLeft.Hours} ساعة و {timeLeft.Minutes} دقيقة.",
                                    "قيود التغيير نشطة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Save the new sleep times
            Properties.Settings.Default.StartTime = selectedStartTime;
            Properties.Settings.Default.EndTime = selectedEndTime;

            // Save the new restriction period
            int newRestrictionHours = (int)numericUpDownRestrictionPeriod.Value;
            Properties.Settings.Default.RestrictionPeriod = newRestrictionHours;

            // Update the last settings change time
            Properties.Settings.Default.LastSettingsChangeTime = DateTime.Now;

            // Save the RunOnStartup setting
            Properties.Settings.Default.RunOnStartup = checkBoxStartup.Checked;

            // Update the registry for RunOnStartup
            string appPath = Application.ExecutablePath;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (checkBoxStartup.Checked)
                    {
                        key.SetValue("SleepSchedulerApp", $"\"{appPath}\"");
                    }
                    else
                    {
                        key.DeleteValue("SleepSchedulerApp", false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحديث إعدادات بدء التشغيل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Save all settings
            Properties.Settings.Default.Save();

            MessageBox.Show("تم حفظ الإعدادات بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Update control states based on the new restriction period
            DisableControlsIfRestrictionActive();

            // Update the last change time label
            labelLastChangeTime.Text = $"تم تعديل الإعدادات آخر مرة في: {DateTime.Now}";

            // Set status message
            labelStatus.Text = "تم حفظ الإعدادات بنجاح.";
            statusClearTimer.Start();

            // Call CheckSleepTime to set up new timers
            CheckSleepTime();
        }

        private void CheckSleepTime()
        {
            // Dispose existing timers to avoid overlaps
            DisposeTimer(ref preSleepTimer);

            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

            // Handle overnight sleep schedule
            if (selectedEndTime < selectedStartTime)
            {
                todayEnd = todayEnd.AddDays(1); // End time moves to the next day
            }

            // Case 1: Within sleep time
            if (now >= todayStart && now <= todayEnd)
            {
                LogEvent("Current time is within sleep time. Initiating countdown and shutdown.");
                ShowCountdownAndShutdown();
            }
            // Case 2: Before sleep time
            else if (now < todayStart)
            {
                TimeSpan timeUntilSleep = todayStart - now;

                if (timeUntilSleep.TotalMilliseconds > 5 * 60 * 1000)
                {
                    // Set a timer to trigger the 5-minute warning
                    preSleepTimer = new Timer
                    {
                        Interval = (int)(timeUntilSleep.TotalMilliseconds - (5 * 60 * 1000)) // Trigger exactly 5 minutes before sleep
                    };

                    preSleepTimer.Tick += (sender, e) =>
                    {
                        DisposeTimer(ref preSleepTimer);
                        LogEvent("5-minute alert triggered.");
                        ShowReminderBeforeSleep();
                    };
                    preSleepTimer.Start();
                }
                else
                {
                    // Less than 5 minutes until sleep
                    LogEvent("Less than 5 minutes to sleep time. Starting immediate countdown.");
                    ShowCountdownBeforeSleep((int)timeUntilSleep.TotalSeconds);
                }
            }
            // Case 3: After sleep time (next day or missed schedule)
            else
            {
                LogEvent("Current time is past sleep time. No action required.");
            }
        }

        //private void ShowReminderBeforeSleep()
        //{
        //    try
        //    {
        //        LogEvent("Displaying reminder to user that shutdown will occur in 5 minutes.");
        //        MessageBox.Show("Your computer will shut down in 5 minutes. Please save your work and prepare to rest.",
        //            "Sleep Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        // Start a countdown for 5 minutes
        //        ShowCountdownBeforeSleep(5 * 60);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogEvent($"Error in ShowReminderBeforeSleep: {ex.Message}");
        //    }
        //}
        private void ShowReminderBeforeSleep()
        {
            try
            {
                LogEvent("Displaying reminder with countdown for 5 minutes before shutdown.");

                // Show a countdown for 5 minutes
                int countdownTime = 5 * 60; // 5 minutes in seconds
                CountdownForm countdownForm = new CountdownForm(countdownTime); // Non-cancellable countdown
                countdownForm.ShowDialog();

                LogEvent("Countdown finished. Preparing for shutdown.");
                ShutdownComputer();
            }
            catch (Exception ex)
            {
                LogEvent($"Error in ShowReminderBeforeSleep: {ex.Message}");
            }
        }


        private void ShowCountdownBeforeSleep(int remainingSeconds)
        {
            try
            {
                LogEvent($"Starting countdown for {remainingSeconds} seconds.");

                // Display a countdown form for the remaining time
                CountdownForm countdownForm = new CountdownForm(remainingSeconds); // Non-cancellable countdown
                countdownForm.ShowDialog();

                LogEvent("Countdown completed. Preparing to shut down.");
                ShutdownComputer();
            }
            catch (Exception ex)
            {
                LogEvent($"Error in ShowCountdownBeforeSleep: {ex.Message}");
            }
        }



        private void DisposeTimer(ref Timer timer)
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null; // Set to null to avoid accessing disposed timer
                }
            }
            catch (Exception ex)
            {
                LogEvent($"Error while disposing timer: {ex.Message}");
            }
        }


        private void ShowCountdownAndShutdown()
        {
            LogEvent("بدأ وقت النوم. يتم التحضير لإيقاف تشغيل الكمبيوتر.");

            // Show a non-cancellable countdown
            CountdownForm countdownForm = new CountdownForm(10);
            countdownForm.ShowDialog();

            ShutdownComputer();
        }

        private void ShutdownComputer()
        {
            try
            {
                LogEvent("محاولة إيقاف تشغيل الكمبيوتر.");
                System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
                LogEvent("تم إصدار أمر إيقاف تشغيل الكمبيوتر بنجاح.");
            }
            catch (Exception ex)
            {
                LogEvent($"فشل في إيقاف تشغيل الكمبيوتر: {ex.Message}");
                MessageBox.Show($"فشل في إيقاف تشغيل الكمبيوتر: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogEvent(string message)
        {
            try
            {
                string logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string logFilePath = System.IO.Path.Combine(logDirectory, "SleepSchedulerLog.txt");

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
                }
            }
            catch
            {
                // Optionally, you can ignore logging failures
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                LogEvent("قام المستخدم بإلغاء قفل الجلسة. يتم التحقق من وقت النوم.");

                DateTime now = DateTime.Now;
                DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
                DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

                if (now >= todayStart && now <= todayEnd)
                {
                    // It's still sleep time; enforcing shutdown
                    ShowCountdownAndShutdown();
                }
                else
                {
                    LogEvent("انتهى وقت النوم. لم يتم اتخاذ أي إجراء.");
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            restrictionCheckTimer?.Stop();
            restrictionCheckTimer?.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

            if (now >= todayStart && now <= todayEnd)
            {
                // Prevent closing during sleep time
                e.Cancel = true;
                MessageBox.Show("لا يمكنك إغلاق التطبيق خلال وقت النوم.", "جدولة النوم", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("هل أنت متأكد أنك تريد إغلاق التطبيق؟", "تأكيد الإغلاق", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            // Save settings on close
            Properties.Settings.Default.Save();
        }

        private void DisableControlsIfRestrictionActive()
        {
            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            int savedRestrictionHours = Properties.Settings.Default.RestrictionPeriod;

            if (savedRestrictionHours > 0)
            {
                // Calculate restriction start time: X hours before the sleep start time
                DateTime restrictionStartTime = todayStart.AddHours(-savedRestrictionHours);

                // Check if we are currently within the restriction period (before sleep time)
                if (now >= restrictionStartTime && now < todayStart)
                {
                    // Restriction is active: disable controls
                    DisableControls();

                    // Update label to show restriction status
                    labelRestrictionInfo.Text = $"الإعدادات مقيدة حتى: {todayStart}";
                    labelRestrictionInfo.Visible = true;

                    // Start the timer to keep checking for the restriction period
                    if (!restrictionCheckTimer.Enabled)
                    {
                        restrictionCheckTimer.Start();
                    }
                }
                else
                {
                    // Restriction is not active, enable controls
                    EnableControls();

                    // Hide the restriction info label
                    labelRestrictionInfo.Visible = false;

                    // Stop the timer since restriction has ended
                    if (restrictionCheckTimer.Enabled)
                    {
                        restrictionCheckTimer.Stop();
                    }
                }
            }
            else
            {
                // No active restriction, enable controls
                numericUpDownRestrictionPeriod.Enabled = true;
                dateTimePickerStart.Enabled = true;
                dateTimePickerEnd.Enabled = true;
                checkBoxStartup.Enabled = true;

                // Hide the restriction info label
                labelRestrictionInfo.Visible = false;

                // Stop the timer since there's no restriction
                if (restrictionCheckTimer.Enabled)
                {
                    restrictionCheckTimer.Stop();
                }
            }
        }

        private void DisableControls()
        {
            numericUpDownRestrictionPeriod.Enabled = false;
            dateTimePickerStart.Enabled = false;
            dateTimePickerEnd.Enabled = false;
            checkBoxStartup.Enabled = false;
        }

        private void EnableControls()
        {
            numericUpDownRestrictionPeriod.Enabled = true;
            dateTimePickerStart.Enabled = true;
            dateTimePickerEnd.Enabled = true;
            checkBoxStartup.Enabled = true;
        }

        // Optional: Show About Dialog
        private void ShowAboutDialog()
        {
            MessageBox.Show("تطبيق جدولة النوم\nالإصدار 1.0.0\nحقوق النشر © 2023 \nالمطور د.محمد قاسم", "حول التطبيق", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}