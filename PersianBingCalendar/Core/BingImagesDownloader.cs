using System;
using System.IO;
using System.Linq;
using System.Text;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;
using System.Net.Http;

namespace PersianBingCalendar.Core
{
    public static class BingImagesDownloader
    {
        public static void DownloadTodayBingImages()
        {
            if (!NetworkStatus.IsConnectedToInternet())
            {
                "The internet connection was not found.".LogText();
                return;
            }

            var dir = DirUtils.GetImagesDir();
            Enumerable.Range(start: 0, count: 9).AsParallel().ForAll(imageIndex =>
            {
                try
                {
                    var source = $"http://www.bing.com/HPImageArchive.aspx?format=xml&idx={imageIndex}&n=1&mkt=en-US";
                    downloadImage(source, dir, xml =>
                    {
                        var bingImage = xml.FromXmlContent<images>();
                        return new ImageInfo
                        {
                            Url = $"http://www.bing.com{bingImage.image.url.Replace("_1366x768.jpg", "_1920x1080.jpg")}",
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
            string xmlUrl,
            string dir,
            Func<string, ImageInfo> getFullUrl)
        {
            string xmlData;
            var timeout = TimeSpan.FromMinutes(5);
            using (var xmlClient = new MyWebClient { Timeout = timeout, Encoding = Encoding.UTF8 })
            {
                Console.WriteLine($"Downloading {xmlUrl}");
                xmlData = xmlClient.DownloadString(xmlUrl);
            }

            var imageInfo = getFullUrl(xmlData);

            var imageName = getImageName(imageInfo);
            var imagePath = Path.Combine(dir, $"{imageName}");

            var xmlFileName = imageName.Split('_').First();
            var xmlFilePath = Path.Combine(dir, $"{xmlFileName}.xml");

            if (File.Exists(xmlFilePath) && File.Exists(imagePath))
            {
                Console.WriteLine($"Already have it: {xmlFileName}");
                return;
            }
            File.WriteAllText(xmlFilePath, xmlData);

            try
            {
                Downloader.DownloadFile(imageInfo.Url, imagePath);
            }
            catch (Exception ex)
            {
                ex.LogException();
                File.Delete(imagePath);
            }
        }

        private static string getImageName(ImageInfo imageInfo)
        {
            var imageName = Path.GetFileName(imageInfo.Url).CleanFileName();
            if (imageName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                return imageName;
            }

            var nameValueCollection = new Uri(imageInfo.Url).ParseQueryString();
            var id = nameValueCollection["id"];
            return string.IsNullOrWhiteSpace(id) ? imageName : id.CleanFileName();
        }
    }
}