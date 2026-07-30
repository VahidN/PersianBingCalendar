using System;
using System.IO;
using System.Security.Cryptography;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core
{
    public static class HolidaysUpdater
    {
        private const string HolidaysUrl =
            "https://raw.githubusercontent.com/VahidN/PersianBingCalendar/master/PersianBingCalendar/bin/Release/Data/holidays.csv";

        private const string HolidaysFileName = "holidays.csv";
        private const string LastCheckFileName = "holidays-last-check.txt";
        private const string TempFileName = "holidays.csv.tmp";

        public static void UpdateIfNeeded()
        {
            try
            {
                var dataDir = Path.Combine(DirUtils.GetAppPath(), "data");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                var lastCheckPath = Path.Combine(dataDir, LastCheckFileName);
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                if (File.Exists(lastCheckPath) &&
                    string.Equals(File.ReadAllText(lastCheckPath).Trim(), today, StringComparison.Ordinal))
                {
                    return;
                }

                if (!NetworkStatus.IsConnectedToInternet())
                {
                    return;
                }

                var localPath = Path.Combine(dataDir, HolidaysFileName);
                var tempPath = Path.Combine(dataDir, TempFileName);

                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }

                    Downloader.DownloadFile(HolidaysUrl, tempPath);

                    if (!File.Exists(tempPath) || new FileInfo(tempPath).Length == 0)
                    {
                        return;
                    }

                    if (!File.Exists(localPath) || !filesHaveSameHash(localPath, tempPath))
                    {
                        if (File.Exists(localPath))
                        {
                            File.Delete(localPath);
                        }
                        File.Move(tempPath, localPath);
                        "holidays.csv updated from remote.".LogText();
                    }
                    else
                    {
                        File.Delete(tempPath);
                    }

                    File.WriteAllText(lastCheckPath, today);
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        try { File.Delete(tempPath); } catch { /* ponytail: best-effort cleanup */ }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static bool filesHaveSameHash(string path1, string path2)
        {
            using (var sha1 = SHA1.Create())
            {
                return hashesEqual(computeHash(sha1, path1), computeHash(sha1, path2));
            }
        }

        private static byte[] computeHash(SHA1 sha1, string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return sha1.ComputeHash(stream);
            }
        }

        private static bool hashesEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
