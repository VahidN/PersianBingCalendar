using System;
using System.IO;
using System.Net;

namespace PersianBingCalendar.Utils;

public static class Downloader
{
	public const string UA = "PersianBingCalendar";

	public static void DownloadFile(string url, string filePath)
	{
		if (string.IsNullOrWhiteSpace(value: url))
		{
			return;
		}

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString: url);
		request.UserAgent = UA;

		request.AllowAutoRedirect = true;
		request.KeepAlive         = false;

		int timeout = (int)TimeSpan.FromMinutes(value: 10).TotalMilliseconds;
		request.Timeout          = timeout;
		request.ReadWriteTimeout = timeout;

		using HttpWebResponse webResponse    = (HttpWebResponse)request.GetResponse();
		using Stream          responseStream = webResponse.GetResponseStream();
		if (responseStream == null) return;
		using FileStream fileStream = new(path: filePath, mode: FileMode.Create,
			access: FileAccess.Write, share: FileShare.None);
		byte[] buffer = new byte[8192];
		int    bufLen = buffer.Length;
		int    readSize;
		while ((readSize = responseStream.Read(buffer: buffer, offset: 0, count: bufLen)) > 0)
		{
			fileStream.Write(array: buffer, offset: 0, count: readSize);
			fileStream.Flush();
		}
	}
}