using System;
using System.IO;
using System.Windows.Forms;
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
			ExecAction execAction = new(path: Application.ExecutablePath, arguments: null,
				workingDirectory: Application.StartupPath);
			using (TaskDefinition task = TaskService.Instance.NewTask())
			{
				bool isVesrion2 = TaskService.Instance.HighestSupportedVersion >= new Version(major: 1, minor: 2);

				task.RegistrationInfo.Description = description;
				task.RegistrationInfo.Author      = "DNT";
				task.Settings.Hidden =
					true; // Project -> Properties -> Application tab -> Output type -> `Windows Application`
				task.Settings.Enabled                    = true;
				task.Settings.DisallowStartIfOnBatteries = false;
				task.Settings.StopIfGoingOnBatteries     = false;
				if (isVesrion2)
				{
					task.Settings.StartWhenAvailable = true;
					task.Settings.MultipleInstances  = TaskInstancesPolicy.IgnoreNew;
				}

				TimeTrigger trigger1 = createHourlyTrigger();
				if (!isVesrion2)
				{
					trigger1.Repetition.Duration = TimeSpan.FromHours(value: 1.1);
				}

				task.Triggers.Add(unboundTrigger: trigger1);

				TimeTrigger trigger2 = createDailyStartTrigger();
				if (!isVesrion2)
				{
					trigger2.Repetition.Duration = TimeSpan.FromHours(value: 1.1);
				}

				task.Triggers.Add(unboundTrigger: trigger2);

				task.Actions.Add(action: execAction);
				TaskService.Instance.RootFolder.RegisterTaskDefinition(
					path: description,
					definition: task,
					createType: TaskCreation.CreateOrUpdate,
					userId: null,
					password: null,
					logonType: TaskLogonType.InteractiveToken);
			}

			Console.WriteLine(value: $"`{description}` task has been added.");
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	public static void RunTask()
	{
		File.WriteAllText(path: Path.Combine(path1: DirUtils.GetAppPath(), path2: "last-run.log"),
			contents: string.Format(format: "{0}{1}{1}", arg0: DateTime.Now, arg1: Environment.NewLine));
		BingImagesDownloader.DownloadTodayBingImages();
		BingWallpaper.SetTodayWallpapaer();
	}

	private static TimeTrigger createHourlyTrigger() =>
		new()
		{
			StartBoundary = new(year: 2017, month: 1, day: 1),
			Repetition =
			{
				Interval          = TimeSpan.FromHours(value: 1),
				StopAtDurationEnd = false
			},
			Enabled = true
		};

	private static TimeTrigger createDailyStartTrigger()
	{
		DateTime tomorrow = DateTime.Now.AddDays(value: 1);
		DateTime startOfADay = new(year: tomorrow.Year, month: tomorrow.Month, day: tomorrow.Day, hour: 0, minute: 0,
			second: 1);
		return new(startBoundary: startOfADay);
	}
}