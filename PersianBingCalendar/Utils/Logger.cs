using System;
using System.IO;

namespace PersianBingCalendar.Utils
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void LogException(this Exception ex)
        {
            ex?.ToString().LogText();
        }

        public static void LogText(this string text)
        {
            lock (_lock)
            {
                Console.WriteLine(text);
                File.AppendAllText(
                    Path.Combine(DirUtils.GetAppPath(), "errors.log"),
                    $"{DateTime.Now}{Environment.NewLine}{text}{Environment.NewLine}{Environment.NewLine}");
            }
        }
    }
}