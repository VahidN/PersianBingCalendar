using System.Globalization;
using Microsoft.Win32.TaskScheduler;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class TaskRunner
{
    public static void CreateOrUpdateTask()
    {
        try
        {
            const string description = "PersianBingCalendar Runner";
            var execAction = new ExecAction(Application.ExecutablePath, arguments: null, Application.StartupPath);

            using (var task = TaskService.Instance.NewTask())
            {
                var isV2 = TaskService.Instance.HighestSupportedVersion >= new Version(major: 1, minor: 2);

                task.RegistrationInfo.Description = description;
                task.RegistrationInfo.Author = "DNT";

                task.Settings.Hidden =
                    true; // Project -> Properties -> Application tab -> Output type -> `Windows Application`

                task.Settings.Enabled = true;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.StopIfGoingOnBatteries = false;

                if (isV2)
                {
                    task.Settings.StartWhenAvailable = true;
                    task.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                }

                var trigger1 = CreateHourlyTrigger();

                if (!isV2)
                {
                    trigger1.Repetition.Duration = TimeSpan.FromHours(value: 1.1);
                }

                task.Triggers.Add(trigger1);

                var trigger2 = CreateDailyStartTrigger();

                if (!isV2)
                {
                    trigger2.Repetition.Duration = TimeSpan.FromHours(value: 1.1);
                }

                task.Triggers.Add(trigger2);

                task.Actions.Add(execAction);

                TaskService.Instance.RootFolder.RegisterTaskDefinition(description, task, TaskCreation.CreateOrUpdate,
                    userId: null, password: null, TaskLogonType.InteractiveToken, sddl: null);
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
        File.WriteAllText(Path.Combine(DirUtils.GetAppPath(), path2: "last-run.log"),
            string.Format(CultureInfo.InvariantCulture, format: "{0}{1}{1}", DateTime.Now, Environment.NewLine));

        HolidaysUpdater.TryUpdateDataFileIfNeeded();
        BingImagesDownloader.DownloadTodayBingImages();
        BingWallpaper.SetTodayWallPaper();
    }

    private static TimeTrigger CreateHourlyTrigger()
        => new()
        {
            StartBoundary = new DateTime(year: 2017, month: 1, day: 1),
            Repetition =
            {
                Interval = TimeSpan.FromHours(value: 1),
                StopAtDurationEnd = false
            },
            Enabled = true
        };

    private static TimeTrigger CreateDailyStartTrigger()
    {
        var tomorrow = DateTime.Now.AddDays(value: 1);
        var startOfADay = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, hour: 0, minute: 0, second: 1);

        return new TimeTrigger(startOfADay);
    }
}
