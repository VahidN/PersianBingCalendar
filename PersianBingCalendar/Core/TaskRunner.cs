using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core
{
    public static class TaskRunner
    {
        public static void CreateOrUpdateTask()
        {
            try
            {
                const string description = "PersianBingCalendar Runner";
                var execAction = new ExecAction(Application.ExecutablePath, null, Application.StartupPath);
                using (var task = TaskService.Instance.NewTask())
                {
                    var isVesrion2 = TaskService.Instance.HighestSupportedVersion >= new Version(1, 2);

                    task.RegistrationInfo.Description = description;
                    task.RegistrationInfo.Author = "DNT";
                    task.Settings.Hidden = true; // Project -> Properties -> Application tab -> Output type -> `Windows Application`
                    task.Settings.Enabled = true;
                    task.Settings.DisallowStartIfOnBatteries = false;
                    task.Settings.StopIfGoingOnBatteries = false;
                    if (isVesrion2)
                    {
                        task.Settings.StartWhenAvailable = true;
                        task.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                    }

                    var trigger1 = createHourlyTrigger();
                    if (!isVesrion2)
                    {
                        trigger1.Repetition.Duration = TimeSpan.FromHours(1.1);
                    }
                    task.Triggers.Add(trigger1);

                    var trigger2 = createDailyStartTrigger();
                    if (!isVesrion2)
                    {
                        trigger2.Repetition.Duration = TimeSpan.FromHours(1.1);
                    }
                    task.Triggers.Add(trigger2);

                    task.Actions.Add(execAction);
                    TaskService.Instance.RootFolder.RegisterTaskDefinition(
                        description,
                        task,
                        TaskCreation.CreateOrUpdate,
                        null,
                        null,
                        TaskLogonType.InteractiveToken,
                        null);
                }
                Console.WriteLine($"`{description}` task has been added.");
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void RunTask()
        {
            File.WriteAllText(Path.Combine(DirUtils.GetAppPath(), "last-run.log"), string.Format("{0}{1}{1}", DateTime.Now, Environment.NewLine));
            BingImagesDownloader.DownloadTodayBingImages();
            BingWallpaper.SetTodayWallpapaer();
        }

        private static TimeTrigger createHourlyTrigger()
        {
            return new TimeTrigger
            {
                StartBoundary = new DateTime(2017, 1, 1),
                Repetition =
                {
                    Interval = TimeSpan.FromHours(1),
                    StopAtDurationEnd = false
                },
                Enabled = true
            };
        }

        private static TimeTrigger createDailyStartTrigger()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var startOfADay = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 1);
            return new TimeTrigger(startOfADay);
        }
    }
}