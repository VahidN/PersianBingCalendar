using System;
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
                ex.LogException();
            }
        }
    }
}