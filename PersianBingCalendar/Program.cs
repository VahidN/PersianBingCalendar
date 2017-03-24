using System;
using System.IO;
using System.Runtime.ExceptionServices;
using PersianBingCalendar.Core;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar
{
    class Program
    {
        [HandleProcessCorruptedStateExceptions]
        static void Main(string[] args)
        {
            try
            {
                RunOnWindowsStartup.Do();
                TaskRunner.CreateOrUpdateTask();
                TaskRunner.RunTask();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                File.AppendAllText(Path.Combine(DirUtils.GetAppPath(), "errors.log"), string.Format("{0}{1}{1}", ex, Environment.NewLine));
            }
        }
    }
}