using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;

class IpRangeInfo
{
    public BigInteger Start;
    public BigInteger End;
    public string CountryCode;
    public string CountryName;
    public string StateCode;
    public string StateName;
}

class Program
{
    static List<IpRangeInfo> ipv4Ranges = new List<IpRangeInfo>();
    static List<IpRangeInfo> ipv6Ranges = new List<IpRangeInfo>();

    static void LoadDatabase(string filePath)
    {
        // Для примера — парсинг CSV
        foreach (var line in File.ReadLines(filePath))
        {
            var parts = line.Split(','); // форматировать под ваш CSV
            var range = new IpRangeInfo
            {
                Start = ParseIp(parts[0]),
                End = ParseIp(parts[1]),
                CountryCode = parts[2],
                CountryName = parts[3],
                StateCode = parts[4],
                StateName = parts[5]
            };
            if (parts[0].Contains(":"))
                ipv6Ranges.Add(range);
            else
                ipv4Ranges.Add(range);
        }
        ipv4Ranges.Sort((a, b) => a.Start.CompareTo(b.Start));
        ipv6Ranges.Sort((a, b) => a.Start.CompareTo(b.Start));
    }

    static BigInteger ParseIp(string ip)
    {
        if (ip.Contains(":"))
            return new BigInteger(IPAddress.Parse(ip).GetAddressBytes(), isUnsigned: true, isBigEndian: true);
        else
            return new BigInteger(IPAddress.Parse(ip).GetAddressBytes().Reverse().ToArray());
    }

    static IpRangeInfo FindIpInfo(string ip)
    {
        var value = ParseIp(ip);
        var ranges = ip.Contains(":") ? ipv6Ranges : ipv4Ranges;
        int left = 0, right = ranges.Count - 1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (value < ranges[mid].Start)
            {
                right = mid - 1;
            }
            else if (value > ranges[mid].End)
            {
                left = mid + 1;
            }
            else
            {
                return ranges[mid];
            }
        }
        return null;
    }

    static void Main()
    {
        LoadDatabase("db.csv");
        string ip = "8.8.8.8";
        var info = FindIpInfo(ip);
        if (info != null)
        {
            Console.WriteLine($"{info.CountryCode},{info.CountryName},{info.StateCode},{info.StateName}");
        }
        else
        {
            Console.WriteLine("Not found");
        }
    }
}
