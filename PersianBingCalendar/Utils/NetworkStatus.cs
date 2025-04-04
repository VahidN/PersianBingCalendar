using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace PersianBingCalendar.Utils;

public static class NetworkStatus
{
	public static bool IsConnectedToInternet(int timeoutPerHostMillis = 1000, string[] hostsToPing = null)
	{
		bool networkAvailable = NetworkInterface.GetIsNetworkAvailable();
		if (!networkAvailable) return false;

		string[] hosts = hostsToPing ?? ["www.google.com", "http://www.google.com"];

		return canPing(timeoutPerHostMillis: timeoutPerHostMillis, hosts: hosts) || canOpenRead(hosts: hosts);
	}

	private static bool canPing(int timeoutPerHostMillis, IEnumerable<string> hosts)
	{
		using (Ping ping = new())
		{
			foreach (string host in hosts)
			{
				try
				{
					PingReply pingReply = ping.Send(hostNameOrAddress: host, timeout: timeoutPerHostMillis);
					if (pingReply is { Status: IPStatus.Success })
						return true;
				}
				catch
				{
					// ignored
				}
			}
		}

		return false;
	}

	private static bool canOpenRead(IEnumerable<string> hosts)
	{
		foreach (string host in hosts)
		{
			try
			{
				using WebClient webClient = new();
				webClient.Headers.Add(name: "user-agent", value: Downloader.UA);
				using Stream stream = webClient.OpenRead(address: host);
				return true;
			}
			catch
			{
				// ignored
			}
		}

		return false;
	}
}