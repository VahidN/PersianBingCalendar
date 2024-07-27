using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class BingImagesDownloader
{
	public static void DownloadTodayBingImages()
	{
		if (!NetworkStatus.IsConnectedToInternet())
		{
			"The internet connection was not found.".LogText();
			return;
		}

		string dir = DirUtils.GetImagesDir();
		Enumerable.Range(start: 0, count: 9).AsParallel().ForAll(imageIndex =>
		{
			try
			{
				string source =
					$"http://www.bing.com/HPImageArchive.aspx?format=xml&idx={imageIndex}&n=1&mkt=en-US";
				downloadImage(xmlUrl: source, dir: dir, xml =>
				{
					images bingImage = xml.FromXmlContent<images>();
					return new()
					{
						Url =
							$"http://www.bing.com{bingImage.image.url.Replace(oldValue: "_1366x768.jpg", newValue: "_1920x1080.jpg")}",
						Copyright = bingImage.image.copyright
					};
				});
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		});
	}

	private static void downloadImage(
		string                  xmlUrl,
		string                  dir,
		Func<string, ImageInfo> getFullUrl)
	{
		string   xmlData;
		TimeSpan timeout = TimeSpan.FromMinutes(value: 5);
		using (MyWebClient xmlClient = new())
		{
			xmlClient.Timeout  = timeout;
			xmlClient.Encoding = Encoding.UTF8;
			Console.WriteLine(value: $"Downloading {xmlUrl}");
			xmlData = xmlClient.DownloadString(address: xmlUrl);
		}

		ImageInfo imageInfo = getFullUrl(arg: xmlData);

		string imageName = getImageName(imageInfo: imageInfo);
		string imagePath = Path.Combine(path1: dir, path2: $"{imageName}");

		string xmlFileName = imageName.Split('_').First();
		string xmlFilePath = Path.Combine(path1: dir, path2: $"{xmlFileName}.xml");

		if (File.Exists(path: xmlFilePath) && File.Exists(path: imagePath))
		{
			Console.WriteLine(value: $"Already have it: {xmlFileName}");
			return;
		}

		File.WriteAllText(path: xmlFilePath, contents: xmlData);

		try
		{
			Downloader.DownloadFile(url: imageInfo.Url, filePath: imagePath);
		}
		catch (Exception ex)
		{
			ex.LogException();
			File.Delete(path: imagePath);
		}
	}

	private static string getImageName(ImageInfo imageInfo)
	{
		string imageName = Path.GetFileName(path: imageInfo.Url).CleanFileName();
		if (imageName.EndsWith(value: ".jpg", comparisonType: StringComparison.OrdinalIgnoreCase))
		{
			return imageName;
		}

		NameValueCollection nameValueCollection = new Uri(uriString: imageInfo.Url).ParseQueryString();
		string              id                  = nameValueCollection[name: "id"];
		return string.IsNullOrWhiteSpace(value: id) ? imageName : id.CleanFileName();
	}
}