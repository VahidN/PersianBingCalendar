using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PersianBingCalendar.Utils;

public static class NativeMethods
{
    public enum WallpaperStyle
    {
        None,
        Stretched = 2,
        Centered = 1,
        Fit = 6,
        Fill = 10
    }

    public const int HwndBroadcast = 0xffff;
    public const int WmWinIniChange = 0x001A;
    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_SENDWININICHANGE = 0x02;
    private const int SPIF_UPDATEINIFILE = 0x01;

    [DllImport(dllName: "user32.dll")]
    public static extern int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);

    public static void SetLockScreenImage(string wallpaper)
    {
        try
        {
            Registry.SetValue(keyName: "HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization",
                valueName: "LockScreenImage", wallpaper);
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(
                value:
                "Failed to update Policy Override. Ensure this user can write to 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization'.");
        }
    }

    public static void SetWallpaper(string wallpaper,
        WallpaperStyle wallpaperStyle = WallpaperStyle.Stretched,
        bool tileWallpaper = false)
    {
        using (var key = Registry.CurrentUser.OpenSubKey(name: @"Control Panel\Desktop", writable: true))
        {
            key?.SetValue(name: @"WallpaperStyle", ((int)wallpaperStyle).ToString(CultureInfo.InvariantCulture));

            key?.SetValue(name: @"TileWallpaper",
                tileWallpaper ? 1.ToString(CultureInfo.InvariantCulture) : 0.ToString(CultureInfo.InvariantCulture));
        }

        SystemParametersInfo(SPI_SETDESKWALLPAPER, uParam: 0, wallpaper, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }

    [DllImport(dllName: "user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
}
