using PersianBingCalendar.Core;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar;

internal static class Program
{
    [STAThread]
    private static void Main()
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
