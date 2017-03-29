using System;

namespace PersianBingCalendar.Utils
{
    public static class WindowsVersion
    {
        public static bool IsWindows8Plus
        {
            get
            {
                var win8Version = new Version(6, 2, 9200, 0);
                return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                       Environment.OSVersion.Version >= win8Version;
            }
        }
    }
}