using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PersianBingCalendar.Utils
{
    public static class NativeMethods
    {
        public const int HwndBroadcast = 0xffff;
        public const int WmWinIniChange = 0x001A;
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_SENDWININICHANGE = 0x02;
        const int SPIF_UPDATEINIFILE = 0x01;

        public enum WallpaperStyle
        {
            Stretched = 2,
            Centered = 1,
            Tiled = 1,
            Fit = 6,
            Fill = 10
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);

        public static void SetLockScreenImage(string wallpaper)
        {
            try
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization", "LockScreenImage", wallpaper);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Failed to update Policy Override. Ensure this user can write to 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization'.");
            }
        }

        public static void SetWallpaper(
            string wallpaper,
            WallpaperStyle wallpaperStyle = WallpaperStyle.Stretched,
            bool tileWallpaper = false)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                key.SetValue(@"WallpaperStyle", ((int)wallpaperStyle).ToString());
                key.SetValue(@"TileWallpaper", tileWallpaper ? 1.ToString() : 0.ToString());
            }
            SystemParametersInfo(
                    SPI_SETDESKWALLPAPER,
                    0,
                    wallpaper,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}