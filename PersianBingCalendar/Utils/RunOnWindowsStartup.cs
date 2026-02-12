using System.Windows.Forms;
using Microsoft.Win32;

namespace PersianBingCalendar.Utils;

public static class RunOnWindowsStartup
{
	private const string ProgramName = "PersianBingCalendar";

	public static void Do()
	{
		RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(
			name: @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
		rkApp?.SetValue(name: ProgramName, value: Application.ExecutablePath);
	}

	public static void Undo()
	{
		RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(
			name: @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
		rkApp?.DeleteValue(name: ProgramName, throwOnMissingValue: false);
	}
}