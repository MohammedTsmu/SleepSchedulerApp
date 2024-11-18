using Microsoft.Win32.TaskScheduler;
using System;
using System.Windows.Forms;

namespace SleepSchedulerApp
{
    public partial class Form1 : Form
    {
        private DateTime selectedStartTime;
        private DateTime selectedEndTime;
        private bool isRetrying = false;

        public Form1()
        {
            InitializeComponent();

            // Retrieve saved settings
            selectedStartTime = Properties.Settings.Default.StartTime;
            selectedEndTime = Properties.Settings.Default.EndTime;

            dateTimePickerStart.Value = selectedStartTime;
            dateTimePickerEnd.Value = selectedEndTime;
            checkBoxStartup.Checked = Properties.Settings.Default.RunOnStartup;

            // Check sleep time on application startup
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
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
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
            MessageBox.Show($"Start Time: {selectedStartTime}\nEnd Time: {selectedEndTime}");

            if (selectedEndTime <= selectedStartTime)
            {
                MessageBox.Show("End time must be after start time.");
                return;
            }

            // إنشاء مهمة لإيقاف التشغيل عند وقت النوم
            CreateTask("ShutdownPC", selectedStartTime, @"C:\Windows\System32\shutdown.exe", "/s /f /t 0");

            // يمكنك أيضًا إنشاء مهمة أخرى عند وقت الاستيقاظ إذا لزم الأمر
            // CreateTask("RestartPC", selectedEndTime, @"C:\Windows\System32\shutdown.exe", "/r /f /t 0");

            MessageBox.Show("Settings saved successfully.");
        }


        private void CreateTask(string taskName, DateTime triggerTime, string executablePath, string arguments)
        {
            using (TaskService ts = new TaskService())
            {
                try
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = $"Scheduled task for {taskName}";

                    DateTime triggerTimeWithDate = DateTime.Today.AddHours(triggerTime.Hour).AddMinutes(triggerTime.Minute);

                    DailyTrigger trigger = new DailyTrigger
                    {
                        StartBoundary = triggerTimeWithDate,
                        DaysInterval = 1,
                        Enabled = true
                    };
                    td.Triggers.Add(trigger);

                    td.Actions.Add(new ExecAction(executablePath, arguments, null));

                    td.Settings.RunOnlyIfIdle = false;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.StopIfGoingOnBatteries = false;

                    ts.RootFolder.RegisterTaskDefinition($@"SleepScheduler\{taskName}", td);

                    LogEvent($"{taskName} task created successfully.");
                }
                catch (Exception ex)
                {
                    LogEvent($"Error creating {taskName}: {ex.Message}");
                }
            }
        }

        private void CheckSleepTime()
        {
            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

            if (now >= todayStart && now <= todayEnd)
            {
                LogEvent("Current time is within sleep time. Preparing to lock the computer.");
                ShowCountdownAndLock();
            }
            else if (now < todayStart)
            {
                TimeSpan timeUntilSleep = todayStart - now;
                if (timeUntilSleep.TotalMilliseconds > 60 * 1000)
                {
                    Timer countdownTimer = new Timer
                    {
                        Interval = (int)timeUntilSleep.TotalMilliseconds - (60 * 1000)
                    };

                    countdownTimer.Tick += (sender, e) =>
                    {
                        countdownTimer.Stop();
                        countdownTimer.Dispose();
                        LogEvent("One minute before sleep time. Showing countdown window.");
                        ShowCountdownAndLock();
                    };
                    countdownTimer.Start();
                }
                else
                {
                    LogEvent("Less than one minute to sleep time. Showing countdown immediately.");
                    ShowCountdownAndLock();
                }
            }
            else
            {
                LogEvent("Current time is outside sleep time. No action taken.");
            }
        }

        private void ShowCountdownAndLock()
        {
            if (isRetrying)
            {
                LogEvent("Another retry is already in progress. Skipping this attempt.");
                return;
            }

            CountdownForm countdownForm = new CountdownForm(10);
            DialogResult result = countdownForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                LogEvent("Countdown finished. Shutting down the computer.");
                ShutdownComputer();
                isRetrying = false;
            }
            else
            {
                LogEvent("Countdown canceled by the user. Retrying in 1 minute.");
                isRetrying = true;

                Timer retryTimer = new Timer
                {
                    Interval = 1 * 60 * 1000
                };
                retryTimer.Tick += (sender, e) =>
                {
                    retryTimer.Stop();
                    retryTimer.Dispose();
                    isRetrying = false;

                    DateTime now = DateTime.Now;
                    DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
                    DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

                    if (now >= todayStart && now <= todayEnd)
                    {
                        LogEvent("Retrying to shutdown the computer after cancellation.");
                        ShowCountdownAndLock();
                    }
                    else
                    {
                        LogEvent("Retry skipped as the current time is outside sleep time.");
                    }
                };
                retryTimer.Start();
            }
        }


        private void ShutdownComputer()
        {
            try
            {
                LogEvent("Attempting to shutdown the computer.");
                System.Diagnostics.Process.Start(@"C:\Windows\System32\shutdown.exe", "/s /f /t 0"); // إيقاف التشغيل فورًا
                LogEvent("Computer shutdown successfully.");
            }
            catch (Exception ex)
            {
                LogEvent($"Failed to shutdown the computer: {ex.Message}");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to log event: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
