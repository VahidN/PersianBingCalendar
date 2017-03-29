using System;
using System.IO;

namespace PersianBingCalendar.Utils
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void LogException(this Exception ex)
        {
            lock (_lock)
            {
                File.AppendAllText(
                    Path.Combine(DirUtils.GetAppPath(), "errors.log"),
                    $"{DateTime.Now}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
            }
        }
    }
}