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
            var lastDownalodResult = getLastImage(dir);
            var imagePath = lastDownalodResult?.ImageFileName;
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine($"{imagePath} not found.");
                return;
            }

            using (var renderer = new PersianCalendarRenderer(
                imageFileName: imagePath, calendarFontFileName: "irsans.ttf", calendarFontSize: 23)
            {
                Holidays = HolidaysReader.GetHolidays(),
                CopyrightText = lastDownalodResult.Copyright,
                TodayColor = Color.DarkRed,
                CopyrightFontName = "Tahoma",
                CopyrightFontSize = 19
            })
            {
                using (var bitmap = renderer.DrawThisMonthsCalendar())
                {
                    const string wallpaperFileName = "_wallpaper.png";
                    var wallpaper = Path.Combine(dir, wallpaperFileName);
                    bitmap.Save(wallpaper, ImageFormat.Png);

                    NativeMethods.SetWallpaper(wallpaper);
                    NativeMethods.SetLockScreenImage(wallpaper);
                }
            }
        }

        private static LastDownalodResult getLastImage(string dir)
        {
            var images = new DirectoryInfo(dir).GetFiles("*.jpg");
            if (!images.Any())
            {
                return null;
            }

            var lastImage = images.OrderByDescending(fileInfo => fileInfo.LastWriteTime).FirstOrDefault();
            if (lastImage == null)
            {
                return null;
            }

            var image = lastImage.FullName;
            var xmlFile = $"{image}.xml";
            var copyright = "";

            if (!File.Exists(xmlFile))
            {
                xmlFile = $"{image.Split('_').First()}.xml";
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