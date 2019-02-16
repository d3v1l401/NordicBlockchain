using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Nordic.Extensions
{
    // https://stackoverflow.com/questions/2727609/best-way-to-create-ipendpoint-from-string
    // Svek's
    public class IPEndPoint : System.Net.IPEndPoint
    {
        public IPEndPoint(long address, int port) : base(address, port) { }
        public IPEndPoint(IPAddress address, int port) : base(address, port) { }

        public static bool TryParse(string value, out IPEndPoint result)
        {
            if (!Uri.TryCreate($"tcp://{value}", UriKind.Absolute, out Uri uri) ||
                !IPAddress.TryParse(uri.Host, out IPAddress ipAddress) ||
                uri.Port < 0 || uri.Port > 65535)
            {
                result = default(IPEndPoint);
                return false;
            }

            result = new IPEndPoint(ipAddress, uri.Port);
            return true;
        }
    }
}
