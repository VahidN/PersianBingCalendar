using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PersianBingCalendar.Utils;

public static class DirUtils
{
	public static string CleanFileName(this string fileName)
	{
		return Path.GetInvalidFileNameChars().Aggregate(seed: fileName,
			(current, c) => current.Replace(oldValue: c.ToString(), newValue: string.Empty));
	}

	public static string GetAppPath() => Application.StartupPath;

	public static string GetImagesDir()
	{
		string       appPath          = GetAppPath();
		const string bingImagesFolder = "BingImages";
		string       imagesDir        = Path.Combine(path1: appPath, path2: bingImagesFolder);
		if (!Directory.Exists(path: imagesDir))
		{
			Directory.CreateDirectory(path: imagesDir);
		}

		return imagesDir;
	}
}