using Microsoft.Win32;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SleepSchedulerApp
{
    public partial class Form1 : Form
    {
        private DateTime selectedStartTime;
        private DateTime selectedEndTime;

        // Declare timers at class level
        private Timer preSleepTimer;
        private Timer shutdownTimer;

        private DateTime lastKnownSystemTime;
        private string lastKnownTimeZoneId;
        // Add a Timer field
        private Timer timeZoneCheckTimer;


        public Form1()
        {
            InitializeComponent();

            // Retrieve saved settings
            selectedStartTime = Properties.Settings.Default.StartTime;
            selectedEndTime = Properties.Settings.Default.EndTime;

            dateTimePickerStart.Value = selectedStartTime;
            dateTimePickerEnd.Value = selectedEndTime;
            checkBoxStartup.Checked = Properties.Settings.Default.RunOnStartup;

            // **Load the restriction period**
            numericUpDownRestrictionPeriod.Value = Properties.Settings.Default.RestrictionPeriod;


            // **Display the last settings change time**
            DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;
            if (lastChange > DateTime.MinValue)
            {
                labelLastChangeTime.Text = $"تم تعديل الإعدادات آخر مرة في: {lastChange}";
            }
            else
            {
                labelLastChangeTime.Text = "لم يتم تعديل الإعدادات بعد.";
            }


            // Load last known system time and time zone
            lastKnownSystemTime = Properties.Settings.Default.LastKnownSystemTime;
            lastKnownTimeZoneId = Properties.Settings.Default.LastKnownTimeZoneId;
            // Check for system time or time zone changes
            if (lastKnownSystemTime > DateTime.MinValue)
            {
                DetectSystemTimeManipulation();
            }
            else
            {
                // First run, initialize the values
                lastKnownSystemTime = DateTime.Now;
                lastKnownTimeZoneId = TimeZoneInfo.Local.Id;
                Properties.Settings.Default.LastKnownSystemTime = lastKnownSystemTime;
                Properties.Settings.Default.LastKnownTimeZoneId = lastKnownTimeZoneId;
                Properties.Settings.Default.Save();
            }

            // Initialize the timer to check time zone every 5 minutes (or any interval you prefer)
            timeZoneCheckTimer = new Timer
            {
                Interval = 5 * 60 * 1000 // 5 minutes in milliseconds
            };
            timeZoneCheckTimer.Tick += TimeZoneCheckTimer_Tick;
            timeZoneCheckTimer.Start();

            // Subscribe to the SessionSwitch event
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            // Handle FormClosing to prevent closing during sleep time
            this.FormClosing += Form1_FormClosing;

            // Subscribe to the FormClosed event
            this.FormClosed += Form1_FormClosed;

            // Subscribe to system events
            //Subscribe to the system time change event to detect changes during runtime:
            //Subscribe to Time Zone Change Events
            SystemEvents.TimeChanged += SystemEvents_TimeChanged;
            


            CheckSleepTime();
        }

        private void DateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            selectedStartTime = dateTimePickerStart.Value;
            Properties.Settings.Default.StartTime = selectedStartTime;
            Properties.Settings.Default.Save();
        }

        private void DateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            selectedEndTime = dateTimePickerEnd.Value;
            Properties.Settings.Default.EndTime = selectedEndTime;
            Properties.Settings.Default.Save();
        }

        private void checkBoxStartup_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RunOnStartup = checkBoxStartup.Checked;
            Properties.Settings.Default.Save();

            string appPath = Application.ExecutablePath;
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

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            // Detect system time or time zone changes
            DetectSystemTimeManipulation();

            // **Check if the restriction period has passed**
            DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;
            int restrictionHours = (int)numericUpDownRestrictionPeriod.Value;

            if (restrictionHours > 0 && lastChange > DateTime.MinValue)
            {
                DateTime restrictionEndTime = lastChange.AddHours(restrictionHours);

                if (DateTime.Now < restrictionEndTime)
                {
                    TimeSpan timeLeft = restrictionEndTime - DateTime.Now;
                    MessageBox.Show($"لا يمكنك تغيير إعدادات النوم حتى {restrictionEndTime}.\n" +
                                    $"الوقت المتبقي: {timeLeft.Hours} ساعة و {timeLeft.Minutes} دقيقة.",
                                    "قيود التغيير نشطة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Validate that end time is after start time
            if (selectedEndTime <= selectedStartTime)
            {
                MessageBox.Show("يجب أن يكون وقت انتهاء النوم بعد وقت بدء النوم.");
                return;
            }

            // Save the new sleep times
            selectedStartTime = dateTimePickerStart.Value;
            selectedEndTime = dateTimePickerEnd.Value;
            Properties.Settings.Default.StartTime = selectedStartTime;
            Properties.Settings.Default.EndTime = selectedEndTime;

            // **Save the restriction period**
            Properties.Settings.Default.RestrictionPeriod = restrictionHours;

            // **Update the last settings change time**
            Properties.Settings.Default.LastSettingsChangeTime = DateTime.Now;

            Properties.Settings.Default.Save();

            MessageBox.Show("تم حفظ الإعدادات بنجاح.");

            // **Update the last change time label**
            labelLastChangeTime.Text = $"تم تعديل الإعدادات آخر مرة في: {DateTime.Now}";

            // Call CheckSleepTime to set up new timers
            CheckSleepTime();
        }

        private void CheckSleepTime()
        {
            // Dispose existing timers
            preSleepTimer?.Stop();
            preSleepTimer?.Dispose();

            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

            if (now >= todayStart && now <= todayEnd)
            {
                LogEvent("الوقت الحالي ضمن وقت النوم. يتم تطبيق سياسة النوم.");
                ShowCountdownAndShutdown();
            }
            else if (now < todayStart)
            {
                TimeSpan timeUntilSleep = todayStart - now;

                if (timeUntilSleep.TotalMilliseconds > 60 * 1000)
                {
                    preSleepTimer = new Timer
                    {
                        Interval = (int)timeUntilSleep.TotalMilliseconds - (60 * 1000)
                    };

                    preSleepTimer.Tick += (sender, e) =>
                    {
                        preSleepTimer.Stop();
                        preSleepTimer.Dispose();
                        LogEvent("بقيت دقيقة واحدة لوقت النوم. يتم عرض نافذة العد التنازلي.");
                        ShowCountdownBeforeSleep();
                    };
                    preSleepTimer.Start();
                }
                else if (timeUntilSleep.TotalMilliseconds > 0)
                {
                    LogEvent("أقل من دقيقة واحدة لوقت النوم. يتم عرض العد التنازلي فوراً.");
                    ShowCountdownBeforeSleep();
                }
                else
                {
                    LogEvent("الوقت الحالي خارج وقت النوم. لم يتم اتخاذ أي إجراء.");
                }
            }
            else
            {
                LogEvent("الوقت الحالي خارج وقت النوم. لم يتم اتخاذ أي إجراء.");
            }
        }

        private void ShowCountdownBeforeSleep()
        {
            CountdownForm countdownForm = new CountdownForm(10); // 10-second countdown
            DialogResult result = countdownForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                LogEvent("انتهى العد التنازلي قبل وقت النوم. يتم التحضير لإيقاف التشغيل.");

                // Dispose existing shutdown timer
                shutdownTimer?.Stop();
                shutdownTimer?.Dispose();

                shutdownTimer = new Timer
                {
                    Interval = 2000 // 2-second delay
                };

                shutdownTimer.Tick += (sender, e) =>
                {
                    shutdownTimer.Stop();
                    shutdownTimer.Dispose();
                    ShutdownComputer();
                };

                shutdownTimer.Start();
            }
            else
            {
                LogEvent("تم إلغاء العد التنازلي قبل وقت النوم من قبل المستخدم. سيتم إعادة المحاولة بعد 5 دقائق.");

                // Dispose existing pre-sleep retry timer
                preSleepTimer?.Stop();
                preSleepTimer?.Dispose();

                // Set up a retry timer for pre-sleep countdown
                preSleepTimer = new Timer
                {
                    Interval = 5 * 60 * 1000 // Retry in 5 minutes
                };

                preSleepTimer.Tick += (sender, e) =>
                {
                    preSleepTimer.Stop();
                    preSleepTimer.Dispose();

                    DateTime now = DateTime.Now;
                    DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);

                    if (now < todayStart)
                    {
                        LogEvent("إعادة المحاولة للعد التنازلي قبل وقت النوم.");
                        ShowCountdownBeforeSleep();
                    }
                    else
                    {
                        LogEvent("تم تجاوز إعادة المحاولة لأن وقت النوم قد بدأ.");
                    }
                };

                preSleepTimer.Start();
            }
        }

        private void ShowCountdownAndShutdown()
        {
            LogEvent("بدأ وقت النوم. يتم التحضير لإيقاف تشغيل الكمبيوتر.");

            // Show a non-cancellable countdown
            CountdownForm countdownForm = new CountdownForm(10, false);
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

        private void DetectSystemTimeManipulation()
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                string currentTimeZoneId = TimeZoneInfo.Local.Id;

                // Check for significant time change (e.g., more than 5 minutes)
                TimeSpan timeDifference = currentTime - lastKnownSystemTime;
                if (Math.Abs(timeDifference.TotalMinutes) > 5)
                {
                    HandleSystemTimeChange();
                }

                // Update the stored values
                lastKnownSystemTime = currentTime;
                lastKnownTimeZoneId = currentTimeZoneId;

                Properties.Settings.Default.LastKnownSystemTime = lastKnownSystemTime;
                Properties.Settings.Default.LastKnownTimeZoneId = lastKnownTimeZoneId;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                LogEvent($"Error detecting system time manipulation: {ex.Message}");
            }
        }

        private void HandleSystemTimeChange()
        {
            // Reset restriction period or take appropriate action
            MessageBox.Show("تم اكتشاف تغيير في وقت النظام. لا يُسمح بتغيير وقت النظام أثناء تفعيل فترة القيود.",
                            "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Optionally, reset the last settings change time to enforce the restriction period
            Properties.Settings.Default.LastSettingsChangeTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        private void HandleTimeZoneChange()
        {
            // Reset restriction period or take appropriate action
            MessageBox.Show("تم اكتشاف تغيير في المنطقة الزمنية. لا يُسمح بتغيير المنطقة الزمنية أثناء تفعيل فترة القيود.",
                            "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Optionally, reset the last settings change time to enforce the restriction period
            Properties.Settings.Default.LastSettingsChangeTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        // Implement the timer tick event handler
        private void TimeZoneCheckTimer_Tick(object sender, EventArgs e)
        {
            DetectTimeZoneChange();
        }

        // Implement the DetectTimeZoneChange method
        private void DetectTimeZoneChange()
        {
            try
            {
                string currentTimeZoneId = TimeZoneInfo.Local.Id;

                if (!string.Equals(currentTimeZoneId, lastKnownTimeZoneId, StringComparison.OrdinalIgnoreCase))
                {
                    HandleTimeZoneChange();
                }

                // Update the stored time zone ID
                lastKnownTimeZoneId = currentTimeZoneId;
                Properties.Settings.Default.LastKnownTimeZoneId = lastKnownTimeZoneId;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                LogEvent($"Error detecting time zone change: {ex.Message}");
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
            SystemEvents.TimeChanged -= SystemEvents_TimeChanged;

            timeZoneCheckTimer?.Stop();
            timeZoneCheckTimer?.Dispose();
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
            }
        }

        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            DetectSystemTimeManipulation();
        }

        private void SystemEvents_TimeZoneChanged(object sender, EventArgs e)
        {
            DetectSystemTimeManipulation();
        }

    }
}