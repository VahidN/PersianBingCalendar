using System;

namespace PersianBingCalendar.Utils;

public static class WindowsVersion
{
	public static bool IsWindows8Plus
	{
		get
		{
			Version win8Version = new(major: 6, minor: 2, build: 9200, revision: 0);
			return (Environment.OSVersion.Platform == PlatformID.Win32NT) &&
				(Environment.OSVersion.Version     >= win8Version);
		}
	}
}