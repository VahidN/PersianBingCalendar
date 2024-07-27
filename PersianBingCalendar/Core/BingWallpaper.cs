using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class BingWallpaper
{
	public static void SetTodayWallpapaer()
	{
		string             dir                = DirUtils.GetImagesDir();
		LastDownalodResult lastDownalodResult = getImage(dir: dir);
		string             imagePath          = lastDownalodResult?.ImageFileName;
		if (string.IsNullOrWhiteSpace(value: imagePath) || !File.Exists(path: imagePath))
		{
			Console.WriteLine(value: $"{imagePath} not found.");
			return;
		}

		using PersianCalendarRenderer renderer = new(imageFileName: imagePath);
		renderer.Holidays             = HolidaysReader.GetHolidays();
		renderer.CopyrightText        = lastDownalodResult.Copyright;
		renderer.TodayColor           = Color.DarkRed;
		renderer.CopyrightFontName    = AppConfig.CopyrightFontName;
		renderer.CopyrightFontSize    = AppConfig.CopyrightFontSize;
		renderer.CalendarFontFileName = AppConfig.CalendarFontFileName;
		renderer.CalendarFontSize     = AppConfig.CalendarFontSize;
		renderer.HolidaysFontSize     = AppConfig.HolidaysFontSize;
		renderer.ShowPastHolidays     = AppConfig.ShowPastHolidays;
		renderer.ShowCopyright        = AppConfig.ShowCopyright;
		using Image bitmap = renderer.DrawThisMonthsCalendar();
		string      wallpaper;
		if (WindowsVersion.IsWindows8Plus)
		{
			const string wallpaperFileName = "_wallpaper.png";
			wallpaper = Path.Combine(path1: dir, path2: wallpaperFileName);
			bitmap.Save(filename: wallpaper, format: ImageFormat.Png);
		}
		else
		{
			const string wallpaperFileName = "_wallpaper.bmp";
			wallpaper = Path.Combine(path1: dir, path2: wallpaperFileName);
			bitmap.Save(filename: wallpaper, format: ImageFormat.Bmp);
		}

		NativeMethods.SetWallpaper(wallpaper: wallpaper);
		NativeMethods.SetLockScreenImage(wallpaper: wallpaper);
	}

	private static LastDownalodResult getImage(string dir)
	{
		FileInfo[] images = new DirectoryInfo(path: dir).GetFiles(searchPattern: "*.jpg");
		if (!images.Any())
		{
			return null;
		}

		FileInfo lastImage;
		if (AppConfig.UseRandomImages)
		{
			Random rnd = new();
			lastImage = images[rnd.Next(minValue: 0, maxValue: images.Length - 1)];
		}
		else
		{
			lastImage = images.OrderByDescending(fileInfo => fileInfo.LastWriteTime).FirstOrDefault();
			if (lastImage == null)
			{
				return null;
			}
		}

		string image     = lastImage.FullName;
		string xmlFile   = $"{image}.xml";
		string copyright = "";

		if (!File.Exists(path: xmlFile))
		{
			xmlFile = Path.Combine(path1: Path.GetDirectoryName(path: image),
				path2: $"{Path.GetFileName(path: image).Split('_').First()}.xml");
		}

		if (!File.Exists(path: xmlFile))
			return new()
			{
				ImageFileName = image,
				Copyright     = copyright,
				XmlFileName   = ""
			};
		try
		{
			images info = xmlFile.FromXmlFile<images>();
			copyright = info.image.copyright;
		}
		catch
		{
			// ignored
		}

		return new()
		{
			ImageFileName = image,
			Copyright     = copyright,
			XmlFileName   = ""
		};
	}
}