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

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        private bool isRestoringWindow = false; // Flag to prevent recursive calls
        private bool isCountdownActive = false; // Flag to track if a countdown is active

        private bool isFirstRun = Properties.Settings.Default.IsFirstRun; // Load the first-run flag
        private int runNumber = Properties.Settings.Default.runNumber; // Load the run-count number flag

        //public Form1()
        //{
        //    InitializeComponent();

        //    // Resize the icon and assign it to the button
        //    Icon saveIcon = Properties.Resources.save;
        //    int desiredSize = 24; // Adjust to your preferred size
        //    Bitmap saveBitmap = new Bitmap(saveIcon.ToBitmap(), new Size(desiredSize, desiredSize));
        //    this.buttonSaveSettings.Image = saveBitmap;
        //    this.buttonSaveSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //    this.buttonSaveSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //    this.buttonSaveSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;


        //    // Retrieve saved settings
        //    selectedStartTime = Properties.Settings.Default.StartTime;
        //    selectedEndTime = Properties.Settings.Default.EndTime;

        //    dateTimePickerStart.Value = selectedStartTime;
        //    dateTimePickerEnd.Value = selectedEndTime;
        //    checkBoxStartup.Checked = Properties.Settings.Default.RunOnStartup;

        //    // Load the restriction period
        //    numericUpDownRestrictionPeriod.Value = (decimal)Properties.Settings.Default.RestrictionPeriod;

        //    // Display the last settings change time
        //    DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;
        //    if (lastChange > DateTime.MinValue)
        //    {
        //        labelLastChangeTime.Text = $"تم تعديل الإعدادات آخر مرة في: {lastChange}";
        //    }
        //    else
        //    {
        //        labelLastChangeTime.Text = "لم يتم تعديل الإعدادات بعد.";
        //    }

        //    // Subscribe to the SessionSwitch event
        //    SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

        //    // Initialize the restrictionCheckTimer but do not start it yet
        //    restrictionCheckTimer = new Timer();
        //    restrictionCheckTimer.Interval = 60 * 1000; // 1 minute
        //    restrictionCheckTimer.Tick += (s, args) =>
        //    {
        //        DisableControlsIfRestrictionActive();
        //    };

        //    // Initialize the statusClearTimer
        //    statusClearTimer = new Timer();
        //    statusClearTimer.Interval = 5000; // 5 seconds
        //    statusClearTimer.Tick += (s, args) =>
        //    {
        //        labelStatus.Text = "";
        //        statusClearTimer.Stop();
        //    };

        //    // Check if the restriction period is active and update controls accordingly
        //    DisableControlsIfRestrictionActive();

        //    // Handle FormClosing to prevent closing during sleep time
        //    this.FormClosing += Form1_FormClosing;

        //    // Subscribe to the FormClosed event
        //    this.FormClosed += Form1_FormClosed;

        //    InitializeTrayIcon();
        //    ConfigureFormBehavior();

        //    this.Opacity = 0; // Make the form invisible
        //    this.ShowInTaskbar = false;
        //    this.Visible = false;

        //    //trayIcon.Text = "Sleep Scheduler 1.0.0";
        //    trayIcon.Text = "Sleep Scheduler - Settings [double click]\nVersion: 1.0.0";

        //    // Show the system tray notification if it's the first time running
        //    if (isFirstRun || runNumber <= 7)
        //    {
        //        int remainingNotifications = 7 - runNumber;

        //        // Show a balloon tip for 10 seconds
        //        trayIcon.ShowBalloonTip(10000, "Sleep Scheduler Running",
        //            $"The application is now running in the system tray. You can access it by clicking the tray icon. " +
        //            $"{(remainingNotifications > 0 ? $"This alert will be shown {remainingNotifications} more time(s)." : "This is the last alert.")}",
        //            ToolTipIcon.Info);

        //        // Update the first run flag and run number
        //        if (isFirstRun)
        //        {
        //            Properties.Settings.Default.IsFirstRun = false;
        //        }

        //        runNumber++;
        //        Properties.Settings.Default.runNumber = runNumber;

        //        // Save updated settings
        //        Properties.Settings.Default.Save();
        //    }



        //    CheckSleepTime();
        //}
        public Form1()
        {
            InitializeComponent();

            // Initialize tray icon first to avoid any potential null access issues
            InitializeTrayIcon();

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

            ConfigureFormBehavior();

            this.Opacity = 0; // Make the form invisible
            this.ShowInTaskbar = false;
            this.Visible = false;

            //trayIcon.Text = "Sleep Scheduler 1.0.0";
            trayIcon.Text = "Sleep Scheduler - Settings [double click]\nVersion: 1.0.0";

            // Show the system tray notification if it's the first time running
            if (isFirstRun || runNumber <= 7)
            {
                int remainingNotifications = 7 - runNumber;

                // Ensure trayIcon is not null before showing the balloon tip
                if (trayIcon != null)
                {
                    // Show a balloon tip for 10 seconds
                    trayIcon.ShowBalloonTip(10000, "Sleep Scheduler Running",
                        $"The application is now running in the system tray. You can access it by clicking the tray icon. " +
                        $"{(remainingNotifications > 0 ? $"This alert will be shown {remainingNotifications} more time(s)." : "This is the last alert.")}",
                        ToolTipIcon.Info);
                }

                // Update the first run flag and run number
                if (isFirstRun)
                {
                    Properties.Settings.Default.IsFirstRun = false;
                }

                runNumber++;
                Properties.Settings.Default.runNumber = runNumber;

                // Save updated settings
                Properties.Settings.Default.Save();
            }

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

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            // Get the new sleep times from the DateTimePicker controls
            selectedStartTime = dateTimePickerStart.Value;
            selectedEndTime = dateTimePickerEnd.Value;

            // Adjust validation to handle overnight sleep times
            if (selectedEndTime <= selectedStartTime && selectedEndTime.TimeOfDay <= selectedStartTime.TimeOfDay)
            {
                // This validation works if the end time is on the same day and earlier than the start time.
                // We need to adjust it to handle the scenario where the sleep end time is logically on the next day.

                // Calculate today and the next day end time scenario
                DateTime todayStart = DateTime.Today.Add(selectedStartTime.TimeOfDay);
                DateTime nextDayEnd = DateTime.Today.AddDays(1).Add(selectedEndTime.TimeOfDay);

                // If the end time is on the next day, the logic should accept it
                if (nextDayEnd <= todayStart)
                {
                    MessageBox.Show("يجب أن يكون وقت انتهاء النوم بعد وقت بدء النوم، أو في اليوم التالي بشكل صحيح.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Save the new sleep times
            Properties.Settings.Default.StartTime = selectedStartTime;
            Properties.Settings.Default.EndTime = selectedEndTime;

            // Save the new restriction period duration set by the user
            int userDefinedRestrictionHours = (int)numericUpDownRestrictionPeriod.Value;
            Properties.Settings.Default.RestrictionPeriod = userDefinedRestrictionHours;

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
                        key.SetValue("SleepScheduler", $"\"{appPath}\"");
                    }
                    else
                    {
                        key.DeleteValue("SleepScheduler", false);
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

            // Set up the restriction period timer to activate restriction at the correct time
            SetRestrictionTimer();
        }


        private void CheckSleepTime()
        {
            // Dispose existing timers to avoid overlaps
            DisposeTimer(ref preSleepTimer);

            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

            if (selectedEndTime <= selectedStartTime)
            {
                todayEnd = todayEnd.AddDays(1); // Consider it as extending into the next day
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
                    preSleepTimer = new Timer
                    {
                        Interval = (int)(timeUntilSleep.TotalMilliseconds - (5 * 60 * 1000)) // Trigger exactly 5 minutes before sleep
                    };
                    preSleepTimer.Tick += (sender, e) =>
                    {
                        DisposeTimer(ref preSleepTimer);
                        ShowReminderBeforeSleep();
                    };
                    preSleepTimer.Start();
                }
                else
                {
                    ShowCountdownBeforeSleep((int)timeUntilSleep.TotalSeconds);
                }
            }
            // Case 3: After sleep time (next day or missed schedule)
            else
            {
                LogEvent("Current time is past sleep time. No action required.");
            }
        }

        private void ShowReminderBeforeSleep()
        {
            try
            {
                LogEvent("Displaying reminder with countdown for 5 minutes before shutdown.");

                // Play sound notification
                System.Media.SystemSounds.Exclamation.Play();

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
            if (timer == null) return;

            try
            {
                timer.Stop();
                timer.Dispose();
                timer = null; // Avoid accessing disposed timer
            }
            catch (Exception ex)
            {
                LogEvent($"Error while disposing timer: {ex.Message}");
            }
        }

        private void ShowCountdownAndShutdown()
        {
            LogEvent("بدأ وقت النوم. يتم التحضير لإيقاف تشغيل الكمبيوتر.");

            // Set countdown as active
            isCountdownActive = true;

            try
            {
                // Show a non-cancellable countdown
                CountdownForm countdownForm = new CountdownForm(5);
                countdownForm.ShowDialog();  // This should be a modal dialog to ensure the user can't avoid the shutdown

                LogEvent("Countdown completed. Preparing to shut down.");
                ShutdownComputer();
            }
            catch (Exception ex)
            {
                LogEvent($"Error in ShowCountdownAndShutdown: {ex.Message}");
            }
            finally
            {
                isCountdownActive = false;  // Reset flag after countdown is complete
            }
        }


        private void ShutdownComputer()
        {
            try
            {
                LogEvent("Attempting to shut down the computer.");
                System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
                LogEvent("Shutdown command issued successfully.");
            }
            catch (Exception ex)
            {
                LogEvent($"Failed to shut down the computer: {ex.Message}");
                MessageBox.Show($"Failed to shut down the computer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                LogEvent("User unlocked the session. Checking sleep time.");

                DateTime now = DateTime.Now;
                DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
                DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);

                // Adjust todayEnd if the sleep period goes overnight
                if (selectedEndTime <= selectedStartTime)
                {
                    todayEnd = todayEnd.AddDays(1);
                }

                if (now >= todayStart && now <= todayEnd)
                {
                    // Still within sleep time; enforcing shutdown
                    ShowCountdownAndShutdown();
                }
                else
                {
                    LogEvent("Sleep time is over. No action required.");
                }
            }
            else if (e.Reason == SessionSwitchReason.SessionLock)
            {
                // Handle session lock events if needed to enforce more controls
                LogEvent("Session locked by user.");
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

        //private void DisableControlsIfRestrictionActive()
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
        //    DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);
        //    int savedRestrictionHours = Properties.Settings.Default.RestrictionPeriod;

        //    // Calculate restriction start time: X hours before the sleep start time
        //    DateTime restrictionStartTime = todayStart.AddHours(-savedRestrictionHours);

        //    // Restriction is active if we are between restriction start and sleep end time
        //    if (now >= restrictionStartTime && now <= todayEnd)
        //    {
        //        SetControlsEnabled(false);

        //        // Update label to show restriction status
        //        labelRestrictionInfo.Text = $"الإعدادات مقيدة حتى: {todayEnd}";
        //        labelRestrictionInfo.Visible = true;

        //        // Start the timer to keep checking until restriction ends
        //        if (!restrictionCheckTimer.Enabled)
        //        {
        //            restrictionCheckTimer.Interval = (int)(todayEnd - now).TotalMilliseconds;
        //            restrictionCheckTimer.Start();
        //        }
        //    }
        //    else
        //    {
        //        // Restriction has ended, re-enable controls and clear restriction-related settings
        //        SetControlsEnabled(true);
        //        labelRestrictionInfo.Visible = false;

        //        // Reset LastSettingsChangeTime to allow new changes
        //        Properties.Settings.Default.LastSettingsChangeTime = DateTime.MinValue;
        //        Properties.Settings.Default.Save();

        //        // Stop the timer since restriction has ended
        //        if (restrictionCheckTimer.Enabled)
        //        {
        //            restrictionCheckTimer.Stop();
        //        }
        //    }
        //}

        private void SetControlsEnabled(bool enabled)
        {
            numericUpDownRestrictionPeriod.Enabled = enabled;
            dateTimePickerStart.Enabled = enabled;
            dateTimePickerEnd.Enabled = enabled;
            checkBoxStartup.Enabled = enabled;
        }

        // Optional: Show About Dialog
        private void ShowAboutDialog()
        {
            MessageBox.Show("تطبيق جدولة النوم\nالإصدار 1.0.0\nحقوق النشر © 2023 \nالمطور د.محمد قاسم", "حول التطبيق", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// Initializes the system tray icon and menu.
        private void InitializeTrayIcon()
        {
            try
            {
                // Create tray menu with options
                trayMenu = new ContextMenuStrip();
                trayMenu.Items.Add("Open Settings", null, (s, e) =>
                {
                    try
                    {
                        ShowMainWindow();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to open settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });

                // Configure tray icon
                trayIcon = new NotifyIcon
                {
                    Text = "Sleep Scheduler",
                    Icon = Properties.Resources.sleep, // Ensure this resource exists
                    ContextMenuStrip = trayMenu,
                    Visible = true
                };

                // Handle double-click to open settings
                trayIcon.DoubleClick += (s, e) => ShowMainWindow();
            }
            catch (Exception ex)
            {
                LogEvent($"Error initializing trayIcon: {ex.Message}");
            }
        }


        /// Configures the behavior of the main window on startup and minimize/close actions.
        private void ConfigureFormBehavior()
        {
            // Start minimized to tray
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;

            // Override form closing and resizing behavior
            this.FormClosing += (s, e) =>
            {
                e.Cancel = true; // Cancel the close action
                MinimizeToTray();
            };

            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    MinimizeToTray();
                }
            };
        }

        /// Shows the main application window, Restore opacity when showing the form.
        private void ShowMainWindow()
        {
            if (isRestoringWindow || isCountdownActive)
            {
                // Do not allow restoring window if a countdown is active to avoid freezing or conflicting actions
                //MessageBox.Show("لا يمكن فتح الإعدادات أثناء العد التنازلي للإغلاق.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                isRestoringWindow = true;
                this.Invoke((MethodInvoker)delegate
                {
                    if (this.WindowState == FormWindowState.Minimized)
                        this.WindowState = FormWindowState.Normal; // Ensure it's not minimized

                    this.Opacity = 1; // Restore visibility
                    this.Visible = true;
                    this.ShowInTaskbar = true;
                    this.BringToFront();
                });
            }
            finally
            {
                isRestoringWindow = false;
            }
        }

        /// Minimizes the application to the system tray.
        private void MinimizeToTray()
        {
            if (isRestoringWindow) return; // Prevent recursive calls

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.ShowInTaskbar = false;
                    this.Visible = false;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while minimizing to the tray: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Exits the application and removes the tray icon.
        private void ExitApplication()
        {
            trayIcon.Visible = false;
            trayIcon.Dispose(); // Explicitly dispose of the NotifyIcon
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Prevent closing and minimize to tray instead
            e.Cancel = true;
            MinimizeToTray();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        private void DisableControlsIfRestrictionActive()
        {
            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);
            int savedRestrictionHours = Properties.Settings.Default.RestrictionPeriod;

            // Adjust todayEnd if the sleep period goes overnight
            if (selectedEndTime <= selectedStartTime)
            {
                todayEnd = todayEnd.AddDays(1);
            }

            // Calculate restriction start time: X hours before the sleep start time
            DateTime restrictionStartTime = todayStart.AddHours(-savedRestrictionHours);
            DateTime lastChange = Properties.Settings.Default.LastSettingsChangeTime;

            // Restriction is active if we are between restriction start and sleep end time
            if (lastChange > DateTime.MinValue)
            {
                // Ensure the restriction end is accurately calculated based on the last settings change
                DateTime restrictionEnd = lastChange.AddHours(savedRestrictionHours);

                if (now < restrictionEnd)
                {
                    SetControlsEnabled(false);

                    // Update label to show restriction status
                    labelRestrictionInfo.Text = $"الإعدادات مقيدة حتى: {todayEnd}";
                    labelRestrictionInfo.Visible = true;

                    toolTip1.SetToolTip(labelRestrictionInfo, "لا يمكن تعديل الإعدادات حتى ينتهي وقت النوم.");
                    trayIcon.Text = "Sleep Scheduler (Restrictions Active)";

                    LogEvent("Controls disabled due to active restriction period.");
                }
                else
                {
                    SetControlsEnabled(true);
                    labelRestrictionInfo.Visible = false;

                    LogEvent("Controls enabled, restriction period has ended.");

                    // Reset LastSettingsChangeTime to allow new changes
                    Properties.Settings.Default.LastSettingsChangeTime = DateTime.MinValue;
                    Properties.Settings.Default.Save();
                }
            }
        }

        // SetRestrictionTimer updated to directly invoke EnableRestrictionPeriod if condition is met
        private void SetRestrictionTimer()
        {
            DisposeTimer(ref restrictionCheckTimer);

            DateTime now = DateTime.Now;
            DateTime todayStart = DateTime.Today.AddHours(selectedStartTime.Hour).AddMinutes(selectedStartTime.Minute);
            int userDefinedRestrictionHours = Properties.Settings.Default.RestrictionPeriod;

            DateTime restrictionStartTime = todayStart.AddHours(-userDefinedRestrictionHours);

            if (now < restrictionStartTime)
            {
                TimeSpan timeUntilRestriction = restrictionStartTime - now;

                if (timeUntilRestriction.TotalMilliseconds <= Int32.MaxValue)
                {
                    restrictionCheckTimer = new Timer
                    {
                        Interval = (int)timeUntilRestriction.TotalMilliseconds
                    };
                    restrictionCheckTimer.Tick += (sender, e) =>
                    {
                        DisposeTimer(ref restrictionCheckTimer);
                        EnableRestrictionPeriod();
                    };
                    restrictionCheckTimer.Start();

                    LogEvent($"Restriction timer set to activate at: {restrictionStartTime}");
                }
                else
                {
                    // Handle longer intervals by setting up a periodic check every 12 hours
                    LogEvent("Restriction timer interval too long. Setting a periodic check instead.");
                    restrictionCheckTimer = new Timer
                    {
                        Interval = 12 * 60 * 60 * 1000 // 12 hours
                    };
                    restrictionCheckTimer.Tick += (sender, e) =>
                    {
                        if (DateTime.Now >= restrictionStartTime)
                        {
                            DisposeTimer(ref restrictionCheckTimer);
                            EnableRestrictionPeriod();
                        }
                    };
                    restrictionCheckTimer.Start();
                }
            }
            else if (now >= restrictionStartTime && now < todayStart)
            {
                EnableRestrictionPeriod();
            }
        }

        private void EnableRestrictionPeriod()
        {
            LogEvent("Enabling restriction period.");

            // Disable controls to prevent changes
            SetControlsEnabled(false);

            DateTime todayEnd = DateTime.Today.AddHours(selectedEndTime.Hour).AddMinutes(selectedEndTime.Minute);
            if (selectedEndTime <= selectedStartTime)
            {
                todayEnd = todayEnd.AddDays(1);
            }

            labelRestrictionInfo.Text = $"الإعدادات مقيدة حتى: {todayEnd}";
            labelRestrictionInfo.Visible = true;
            toolTip1.SetToolTip(labelRestrictionInfo, "لا يمكن تعديل الإعدادات حتى ينتهي وقت النوم.");
            trayIcon.Text = "Sleep Scheduler (Restrictions Active)";

            TimeSpan timeUntilEnd = todayEnd - DateTime.Now;

            if (timeUntilEnd.TotalMilliseconds > 0)
            {
                DisposeTimer(ref restrictionCheckTimer);
                restrictionCheckTimer = new Timer
                {
                    Interval = (int)timeUntilEnd.TotalMilliseconds
                };
                restrictionCheckTimer.Tick += (sender, e) =>
                {
                    DisposeTimer(ref restrictionCheckTimer);
                    DisableControlsIfRestrictionActive();
                };
                restrictionCheckTimer.Start();
            }
            else
            {
                LogEvent("Restriction timer not set due to invalid interval. Time until end is negative.");
            }
        }


    }
}