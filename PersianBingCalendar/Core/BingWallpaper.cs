using System.Drawing.Imaging;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class BingWallpaper
{
    public static void SetTodayWallPaper()
    {
        var dir = DirUtils.GetImagesDir();
        var lastDownloadResult = GetImage(dir);
        var imagePath = lastDownloadResult?.ImageFileName;

        if (imagePath is null || string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            $"{imagePath} not found.".LogText();

            return;
        }

        using var renderer = new PersianCalendarRenderer(imagePath);

        renderer.Holidays = HolidaysReader.GetHolidays();
        renderer.CopyrightText = lastDownloadResult?.Copyright ?? "";
        renderer.TodayColor = Color.DarkRed;
        renderer.CopyrightFontName = AppConfig.CopyrightFontName;
        renderer.CopyrightFontSize = AppConfig.CopyrightFontSize;
        renderer.CalendarFontFileName = AppConfig.CalendarFontFileName;
        renderer.CalendarFontSize = AppConfig.CalendarFontSize;
        renderer.HolidaysFontSize = AppConfig.HolidaysFontSize;
        renderer.ShowPastHolidays = AppConfig.ShowPastHolidays;
        renderer.ShowCopyright = AppConfig.ShowCopyright;

        using var bitmap = renderer.DrawThisMonthsCalendar();

        if (bitmap is null)
        {
            return;
        }

        string wallpaper;

        if (WindowsVersion.IsWindows8Plus)
        {
            const string wallpaperFileName = "_wallpaper.png";
            wallpaper = Path.Combine(dir, wallpaperFileName);
            bitmap.Save(wallpaper, ImageFormat.Png);
        }
        else
        {
            const string wallpaperFileName = "_wallpaper.bmp";
            wallpaper = Path.Combine(dir, wallpaperFileName);
            bitmap.Save(wallpaper, ImageFormat.Bmp);
        }

        NativeMethods.SetWallpaper(wallpaper);
        NativeMethods.SetLockScreenImage(wallpaper);
    }

    private static LastDownalodResult? GetImage(string dir)
    {
        var images = new DirectoryInfo(dir).GetFiles(searchPattern: "*.jpg");

        if (images.Length == 0)
        {
            return null;
        }

        FileInfo lastImage;

        if (AppConfig.UseRandomImages)
        {
            var rnd = new Random();
            lastImage = images[rnd.Next(minValue: 0, images.Length - 1)];
        }
        else
        {
            lastImage = images.OrderByDescending(fileInfo => fileInfo.LastWriteTime).FirstOrDefault();

            if (lastImage is null)
            {
                return null;
            }
        }

        var image = lastImage.FullName;
        var xmlFile = $"{image}.xml";
        var copyright = "";

        if (!File.Exists(xmlFile))
        {
            xmlFile = Path.Combine(Path.GetDirectoryName(image), $"{Path.GetFileName(image).Split('_')[0]}.xml");
        }

        if (File.Exists(xmlFile))
        {
            try
            {
                var info = xmlFile.FromXmlFile<images>();
                copyright = info?.image?.copyright;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        return new LastDownalodResult
        {
            ImageFileName = image,
            Copyright = copyright,
            XmlFileName = ""
        };
    }
}
