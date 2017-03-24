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
            const string description = "PersianBingCalendar Runner";
            var execAction = new ExecAction(Application.ExecutablePath, null, Application.StartupPath);

            if (updateTask(description, execAction))
            {
                return;
            }

            addNewTask(description, execAction);
            Console.WriteLine($"`{description}` task has been added.");
        }

        public static void RunTask()
        {
            File.WriteAllText(Path.Combine(DirUtils.GetAppPath(), "last-run.log"), string.Format("{0}{1}{1}", DateTime.Now, Environment.NewLine));
            BingImagesDownloader.DownloadTodayBingImages();
            BingWallpaper.SetTodayWallpapaer();
        }

        private static void addNewTask(string description, ExecAction execAction)
        {
            using (var task = TaskService.Instance.NewTask())
            {
                task.RegistrationInfo.Description = description;
                task.RegistrationInfo.Author = "DNT";
                task.Settings.Hidden = true; // Project -> Properties -> Application tab -> Output type -> `Windows Application`
                task.Settings.Enabled = true;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.StopIfGoingOnBatteries = false;
                task.Settings.StartWhenAvailable = true;
                task.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;

                task.Triggers.Add(createDailyTrigger());
                task.Triggers.Add(createOneTimeTrigger());

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
        }

        private static DailyTrigger createDailyTrigger()
        {
            return new DailyTrigger
            {
                // set start date and time to be in future (near future, just 1 minute), then task will be triggered and will repeat at specific interval
                StartBoundary = DateTime.Now.AddMinutes(1),
                Repetition =
                {
                    Interval = TimeSpan.FromHours(1),
                    StopAtDurationEnd = false
                },
                Enabled = true
            };
        }

        private static TimeTrigger createOneTimeTrigger()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var startOfADay = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 1);
            return new TimeTrigger(startOfADay);
        }

        private static bool updateTask(string description, ExecAction execAction)
        {
            using (var ts = new TaskService())
            {
                var task = ts.GetTask(description);
                if (task != null)
                {
                    task.Definition.Actions[0] = execAction;
                    task.Definition.Triggers[1] = createOneTimeTrigger();
                    task.RegisterChanges();
                    return true;
                }
            }
            return false;
        }
    }
}