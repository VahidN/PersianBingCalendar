using System.Net;
using System.Net.NetworkInformation;

namespace PersianBingCalendar.Utils;

public static class NetworkStatus
{
    public static bool IsConnectedToInternet(int timeoutPerHostMillis = 1000, string[]? hostsToPing = null)
    {
        var networkAvailable = NetworkInterface.GetIsNetworkAvailable();

        if (!networkAvailable)
        {
            return false;
        }

        var hosts = hostsToPing ?? ["www.google.com", "https://www.google.com"];

        return CanPing(timeoutPerHostMillis, hosts) || CanOpenRead(hosts);
    }

    private static bool CanPing(int timeoutPerHostMillis, IEnumerable<string> hosts)
    {
        using var ping = new Ping();

        foreach (var host in hosts)
        {
            try
            {
                var pingReply = ping.Send(host, timeoutPerHostMillis);

                if (pingReply?.Status is IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
            }
        }

        return false;
    }

    private static bool CanOpenRead(IEnumerable<string> hosts)
    {
        foreach (var host in hosts)
        {
            try
            {
                using var webClient = new WebClient();
                webClient.Headers.Add(name: "user-agent", Downloader.UA);
                using var stream = webClient.OpenRead(host);

                return true;
            }
            catch
            {
            }
        }

        return false;
    }
}
