using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace SleepSchedulerApp
{
    public partial class Form1 : Form
    {
        private DateTime selectedStartTime;
        private DateTime selectedEndTime;

        // Declare timers at class level
        private Timer preSleepTimer;
        private Timer shutdownTimer;

        public Form1()
        {
            InitializeComponent();

            // Retrieve saved settings
            selectedStartTime = Properties.Settings.Default.StartTime;
            selectedEndTime = Properties.Settings.Default.EndTime;

            dateTimePickerStart.Value = selectedStartTime;
            dateTimePickerEnd.Value = selectedEndTime;
            checkBoxStartup.Checked = Properties.Settings.Default.RunOnStartup;

            // Subscribe to the SessionSwitch event
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            // Optionally, handle FormClosing to prevent closing during sleep time
            this.FormClosing += Form1_FormClosing;

            // Subscribe to the FormClosed event
            this.FormClosed += Form1_FormClosed;

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

            MessageBox.Show("Settings saved successfully.");

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
                LogEvent("Current time is within sleep time. Enforcing sleep policy.");
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
                        LogEvent("One minute before sleep time. Showing countdown window.");
                        ShowCountdownBeforeSleep();
                    };
                    preSleepTimer.Start();
                }
                else if (timeUntilSleep.TotalMilliseconds > 0)
                {
                    LogEvent("Less than one minute to sleep time. Showing countdown immediately.");
                    ShowCountdownBeforeSleep();
                }
                else
                {
                    LogEvent("Current time is outside sleep time. No action taken.");
                }
            }
            else
            {
                LogEvent("Current time is outside sleep time. No action taken.");
            }
        }

        private void ShowCountdownBeforeSleep()
        {
            CountdownForm countdownForm = new CountdownForm(10); // 10-second countdown
            DialogResult result = countdownForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                LogEvent("Countdown before sleep finished. Preparing to shutdown.");

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
                LogEvent("Countdown before sleep canceled by the user. Will retry in 5 minutes.");

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
                        LogEvent("Retrying pre-sleep countdown.");
                        ShowCountdownBeforeSleep();
                    }
                    else
                    {
                        LogEvent("Pre-sleep retry skipped as sleep time has started.");
                    }
                };

                preSleepTimer.Start();
            }
        }

        private void ShowCountdownAndShutdown()
        {
            LogEvent("Sleep time has started. Preparing to shut down the computer.");

            // Show a non-cancellable countdown
            CountdownForm countdownForm = new CountdownForm(10, false);
            countdownForm.ShowDialog();

            ShutdownComputer();
        }

        private void ShutdownComputer()
        {
            try
            {
                LogEvent("Attempting to shutdown the computer.");
                System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
                LogEvent("Computer shutdown command issued successfully.");
            }
            catch (Exception ex)
            {
                LogEvent($"Failed to shutdown the computer: {ex.Message}");
                MessageBox.Show($"Failed to shutdown the computer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                LogEvent("User has unlocked the session. Checking sleep time.");

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
                    LogEvent("Sleep time has ended. No action taken.");
                }
            }
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
                MessageBox.Show("You cannot close the application during sleep time.", "Sleep Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
