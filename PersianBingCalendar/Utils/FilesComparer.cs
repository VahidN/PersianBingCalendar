using System.Security.Cryptography;

namespace PersianBingCalendar.Utils;

public static class FilesComparer
{
    public static bool HaveFilesSameHash(string path1, string path2)
    {
        if (!File.Exists(path1) || !File.Exists(path2))
        {
            return false;
        }

        using (var sha256 = SHA256.Create())
        {
            var hash1 = ComputeHash(sha256, path1);
            var hash2 = ComputeHash(sha256, path2);

            return FixedTimeEquals(hash1, hash2);
        }
    }

    private static byte[] ComputeHash(SHA256 sha256, string path)
    {
        using (var stream = File.OpenRead(path))
        {
            return sha256.ComputeHash(stream);
        }
    }

    private static bool FixedTimeEquals(byte[]? a, byte[]? b)
    {
        if (a is null || b is null || a.Length != b.Length)
        {
            return false;
        }

        var result = 0;

        for (var i = 0; i < a.Length; i++)
        {
            // در صورت عدم برابری بیت‌ها، result مقدار غیر صفر می‌گیرد
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
