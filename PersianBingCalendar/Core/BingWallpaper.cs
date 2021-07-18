using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core
{
    public static class BingWallpaper
    {
        public static void SetTodayWallpapaer()
        {
            var dir = DirUtils.GetImagesDir();
            var lastDownalodResult = getImage(dir);
            var imagePath = lastDownalodResult?.ImageFileName;
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine($"{imagePath} not found.");
                return;
            }

            using (var renderer = new PersianCalendarRenderer(imageFileName: imagePath)
            {
                Holidays = HolidaysReader.GetHolidays(),
                CopyrightText = lastDownalodResult.Copyright,
                TodayColor = Color.DarkRed,
                CopyrightFontName = AppConfig.CopyrightFontName,
                CopyrightFontSize = AppConfig.CopyrightFontSize,
                CalendarFontFileName = AppConfig.CalendarFontFileName,
                CalendarFontSize = AppConfig.CalendarFontSize,
                HolidaysFontSize = AppConfig.HolidaysFontSize,
                ShowPastHolidays = AppConfig.ShowPastHolidays,
                ShowCopyright = AppConfig.ShowCopyright,
            })
            {
                using (var bitmap = renderer.DrawThisMonthsCalendar())
                {
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
            }
        }

        private static LastDownalodResult getImage(string dir)
        {
            var images = new DirectoryInfo(dir).GetFiles("*.jpg");
            if (!images.Any())
            {
                return null;
            }

            FileInfo lastImage;
            if (AppConfig.UseRandomImages)
            {
                var rnd = new Random();
                lastImage = images[rnd.Next(0, images.Length - 1)];
            }
            else
            {
                lastImage = images.OrderByDescending(fileInfo => fileInfo.LastWriteTime).FirstOrDefault();
                if (lastImage == null)
                {
                    return null;
                }
            }

            var image = lastImage.FullName;
            var xmlFile = $"{image}.xml";
            var copyright = "";

            if (!File.Exists(xmlFile))
            {
                xmlFile = Path.Combine(Path.GetDirectoryName(image), $"{Path.GetFileName(image).Split('_').First()}.xml");
            }

            if (File.Exists(xmlFile))
            {
                try
                {
                    var info = XmlUtils.FromXmlFile<images>(xmlFile);
                    copyright = info.image.copyright;
                }
                catch { }
            }

            return new LastDownalodResult
            {
                ImageFileName = image,
                Copyright = copyright,
                XmlFileName = ""
            };
        }
    }
}