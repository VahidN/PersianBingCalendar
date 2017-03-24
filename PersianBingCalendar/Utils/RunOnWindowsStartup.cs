using System.Windows.Forms;
using Microsoft.Win32;

namespace PersianBingCalendar.Utils
{
    public static class RunOnWindowsStartup
    {
        const string ProgramName = "PersianBingCalendar";

        public static void Do()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.SetValue(ProgramName, Application.ExecutablePath);
        }

        public static void Undo()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.DeleteValue(ProgramName, false);
        }
    }
}