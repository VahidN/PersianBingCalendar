using System;
using System.Net;

namespace PersianBingCalendar.Utils
{
    public class MyWebClient : WebClient
    {
        public TimeSpan Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest request = base.GetWebRequest(uri);
            request.Timeout = (int)Timeout.TotalMilliseconds;
            ((HttpWebRequest)request).ReadWriteTimeout = (int)Timeout.TotalMilliseconds;
            return request;
        }
    }
}