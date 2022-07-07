using System.Net;
using System.Net.Sockets;
namespace CardsOfConflict.Library.Helpers;


public class NetworkHelper
{
    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString() != "127.0.0.1")
            {
                return ip;
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public static IPAddress GetPublicIpAddress()
    {
        string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
        return IPAddress.Parse(externalIpString);
    }
}