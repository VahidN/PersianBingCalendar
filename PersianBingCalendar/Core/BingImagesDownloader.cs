using System.Text;
using System.Web;
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

        var dir = DirUtils.GetImagesDir();

        Enumerable.Range(start: 0, count: 9)
            .AsParallel()
            .ForAll(imageIndex =>
            {
                try
                {
                    var source =
                        FormattableString.Invariant(
                            $"https://www.bing.com/HPImageArchive.aspx?format=xml&idx={imageIndex}&n=1&mkt=en-US");

                    DownloadImage(source, dir, xml =>
                    {
                        var bingImage = xml.FromXmlContent<images>();

                        return new ImageInfo
                        {
                            Url =
                                $"https://www.bing.com{bingImage?.image?.url?.Replace(oldValue: "_1366x768.jpg", newValue: "_1920x1080.jpg")}",
                            Copyright = bingImage?.image?.copyright
                        };
                    });
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            });
    }

    private static void DownloadImage(string xmlUrl, string dir, Func<string, ImageInfo> getFullUrl)
    {
        string xmlData;
        var timeout = TimeSpan.FromMinutes(value: 5);

        using (var xmlClient = new MyWebClient())
        {
            xmlClient.Timeout = timeout;
            xmlClient.Encoding = Encoding.UTF8;
            Console.WriteLine($"Downloading {xmlUrl}");
            xmlData = xmlClient.DownloadString(xmlUrl);
        }

        var imageInfo = getFullUrl(xmlData);

        var imageName = GetImageName(imageInfo);
        var imagePath = Path.Combine(dir, $"{imageName}");

        var xmlFileName = imageName.Split('_')[0];
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

    private static string GetImageName(ImageInfo imageInfo)
    {
        var imageName = Path.GetFileName(imageInfo.Url).CleanFileName();

        if (imageName.EndsWith(value: ".jpg", StringComparison.OrdinalIgnoreCase))
        {
            return imageName;
        }

        var nameValueCollection = HttpUtility.ParseQueryString(new Uri(imageInfo.Url).Query);
        var id = nameValueCollection[name: "id"];

        return string.IsNullOrWhiteSpace(id) ? imageName : id.CleanFileName();
    }
}
