using Microsoft.Win32;

namespace PersianBingCalendar.Utils;

public static class RunOnWindowsStartup
{
    private const string ProgramName = "PersianBingCalendar";

    public static void Do()
    {
        using var rkApp = Registry.CurrentUser.OpenSubKey(name: "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
            writable: true);

        rkApp?.SetValue(ProgramName, Application.ExecutablePath);
    }

    public static void Undo()
    {
        using var rkApp = Registry.CurrentUser.OpenSubKey(name: "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
            writable: true);

        rkApp?.DeleteValue(ProgramName, throwOnMissingValue: false);
    }
}
