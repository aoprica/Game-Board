using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Game.Network
{
    public static class NetUtil
    {
        public static IPAddress GetIP()
        {
            // discover host name;
            // discover an ipv4 belonging to host;
            // return;

            var ipGlobalProps = IPGlobalProperties.GetIPGlobalProperties();
            var hostName = ipGlobalProps.HostName;

            var addrList = Dns.GetHostEntry(hostName).AddressList;

            var internetAddr = addrList
                .Where(x => AddressFamily.InterNetwork == x.AddressFamily)
                .First();

            return internetAddr;
        }
    }
}
