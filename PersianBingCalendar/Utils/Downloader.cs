using System.Net;

namespace PersianBingCalendar.Utils;

public static class Downloader
{
    public static readonly string UA = "PersianBingCalendar";

    static Downloader()
        =>

            // GitHub (and most HTTPS hosts) require TLS 1.2; net40 enum lacks Tls12 (3072).
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

    public static void DownloadFile(string? url, string filePath)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.UserAgent = UA;

        request.AllowAutoRedirect = true;
        request.KeepAlive = false;

        var timeout = (int)TimeSpan.FromMinutes(value: 10).TotalMilliseconds;
        request.Timeout = timeout;
        request.ReadWriteTimeout = timeout;

        using var webResponse = (HttpWebResponse)request.GetResponse();
        using var responseStream = webResponse.GetResponseStream();

        if (responseStream is null)
        {
            return;
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

        var buffer = new byte[8192];
        var bufLen = buffer.Length;
        int readSize;

        while ((readSize = responseStream.Read(buffer, offset: 0, bufLen)) > 0)
        {
            fileStream.Write(buffer, offset: 0, readSize);
            fileStream.Flush();
        }
    }
}
