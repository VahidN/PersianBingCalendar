using System.Net;

namespace PersianBingCalendar.Utils;

public class MyWebClient : WebClient
{
    public TimeSpan Timeout { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = base.GetWebRequest(address);
        request.Timeout = (int)Timeout.TotalMilliseconds;
        ((HttpWebRequest)request).ReadWriteTimeout = (int)Timeout.TotalMilliseconds;

        return request;
    }
}
