namespace PersianBingCalendar.Utils;

public static class DirUtils
{
    public static string CleanFileName(this string fileName)
        => Path.GetInvalidFileNameChars()
            .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));

    public static string GetAppPath() => Application.StartupPath;

    public static string GetImagesDir()
    {
        var appPath = GetAppPath();
        const string bingImagesFolder = "BingImages";
        var imagesDir = Path.Combine(appPath, bingImagesFolder);

        if (!Directory.Exists(imagesDir))
        {
            Directory.CreateDirectory(imagesDir);
        }

        return imagesDir;
    }
}
