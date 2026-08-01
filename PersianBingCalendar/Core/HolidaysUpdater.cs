using System.Globalization;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class HolidaysUpdater
{
    private const string HolidaysUrl =
        "https://raw.githubusercontent.com/VahidN/PersianBingCalendar/master/PersianBingCalendar/bin/Release/Data/holidays.csv";

    private const string HolidaysFileName = "holidays.csv";
    private const string LastCheckFileName = "holidays-last-check.txt";
    private const string TempFileName = "holidays.csv.tmp";

    public static void TryUpdateDataFileIfNeeded()
    {
        try
        {
            if (!NetworkStatus.IsConnectedToInternet())
            {
                return;
            }

            var dataDir = GetDataDir();
            var lastCheckPath = Path.Combine(dataDir, LastCheckFileName);
            var today = DateTime.UtcNow.ToString(format: "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (IsCheckedToday(lastCheckPath, today))
            {
                return;
            }

            var localHolidaysFilePath = Path.Combine(dataDir, HolidaysFileName);
            var downloadedHolidaysFileTempPath = Path.Combine(dataDir, TempFileName);

            try
            {
                TryDeleteFile(downloadedHolidaysFileTempPath);
                Downloader.DownloadFile(HolidaysUrl, downloadedHolidaysFileTempPath);

                if (FileIsNotDownloaded(downloadedHolidaysFileTempPath))
                {
                    return;
                }

                if (!FilesComparer.HaveFilesSameHash(localHolidaysFilePath, downloadedHolidaysFileTempPath))
                {
                    // اوررایت مستقیم بدون ریسک از دست رفتن فایل قبلی در صورت خطا
                    File.Copy(downloadedHolidaysFileTempPath, localHolidaysFilePath, overwrite: true);
                    "holidays.csv file has updated from the remote source.".LogText();
                }

                File.WriteAllText(lastCheckPath, today);
            }
            finally
            {
                TryDeleteFile(downloadedHolidaysFileTempPath);
            }
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    private static bool FileIsNotDownloaded(string tempPath)
        => !File.Exists(tempPath) || new FileInfo(tempPath).Length == 0;

    private static void TryDeleteFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        try
        {
            File.Delete(filePath);
        }
        catch
        {
            /* best-effort cleanup */
        }
    }

    private static bool IsCheckedToday(string lastCheckPath, string today)
        => File.Exists(lastCheckPath) &&
           string.Equals(File.ReadAllText(lastCheckPath).Trim(), today, StringComparison.Ordinal);

    private static string GetDataDir()
    {
        var dataDir = Path.Combine(DirUtils.GetAppPath(), path2: "data");

        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        return dataDir;
    }
}
