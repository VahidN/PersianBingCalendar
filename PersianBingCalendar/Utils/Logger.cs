using System;
using System.IO;

namespace PersianBingCalendar.Utils;

public static class Logger
{
	private static readonly object _lock = new();

	public static void LogException(this Exception ex)
	{
		ex?.ToString().LogText();
	}

	public static void LogText(this string text)
	{
		lock (_lock)
		{
			Console.WriteLine(value: text);
			File.AppendAllText(
				path: Path.Combine(path1: DirUtils.GetAppPath(), path2: "errors.log"),
				contents: $"{DateTime.Now}{Environment.NewLine}{text}{Environment.NewLine}{Environment.NewLine}");
		}
	}
}