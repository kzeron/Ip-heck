using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

class Program
{
    static IPAddress FindMinFreeIP(List<string> usedIps)
    {
        var usedIpSet = new HashSet<uint>(usedIps.Select(IpToUint));
        uint minIp = usedIpSet.Min();
        uint maxIp = usedIpSet.Max();

        for (uint ip = minIp; ip <= maxIp + 1; ip++)
        {
            if ((ip & 0xFF) == 0) 
                continue;
            if (!usedIpSet.Contains(ip))
                return UintToIp(ip);
        }
        return null;
    }

    static uint IpToUint(string ipString)
    {
        var bytes = IPAddress.Parse(ipString).GetAddressBytes();
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    static IPAddress UintToIp(uint ipUint)
    {
        byte[] bytes = BitConverter.GetBytes(ipUint);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return new IPAddress(bytes);
    }

    static void Main()
    {
        var ips = new List<string> { "1.2.3.5", "1.2.3.4", "1.2.3.9" };
        var freeIp = FindMinFreeIP(ips);
        Console.WriteLine(freeIp?.ToString());
    }
}
