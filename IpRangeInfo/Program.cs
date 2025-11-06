using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;

class IpRangeInfo
{
    public BigInteger Start { get; set; }
    public BigInteger End { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string StateCode { get; set; }
    public string StateName { get; set; }
}

class Program
{
    static List<IpRangeInfo> ipv4Ranges = new List<IpRangeInfo>();
    static List<IpRangeInfo> ipv6Ranges = new List<IpRangeInfo>();

    static void LoadDatabase(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл не найден: {filePath}");
            return;
        }

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(';');

            if (parts.Length < 10) continue;

            try
            {
                var cidr = parts[0].Trim();
                var (startIp, endIp) = ParseCidr(cidr);

                var range = new IpRangeInfo
                {
                    Start = startIp,
                    End = endIp,
                    CountryCode = parts[3].Trim(),
                    CountryName = parts[4].Trim(),
                    StateCode = parts[5].Trim(),
                    StateName = parts[6].Trim()
                };

                if (cidr.Contains(":"))
                    ipv6Ranges.Add(range);
                else
                    ipv4Ranges.Add(range);

                // Вывод каждой строки
                Console.WriteLine($"{cidr} -> {range.CountryCode}, {range.CountryName}, {range.StateCode}, {range.StateName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга строки: {line}. {ex.Message}");
            }
        }

        Console.WriteLine($"\nВсего загружено IPv4 диапазонов: {ipv4Ranges.Count}");
        Console.WriteLine($"Всего загружено IPv6 диапазонов: {ipv6Ranges.Count}");
    }

    static (BigInteger start, BigInteger end) ParseCidr(string cidr)
    {
        var parts = cidr.Split('/');
        var ipStr = parts[0];
        var prefixLen = int.Parse(parts[1]);

        var ip = IPAddress.Parse(ipStr);
        var isIPv6 = ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;

        BigInteger ipNum = ParseIp(ipStr);
        int totalBits = isIPv6 ? 128 : 32;
        int hostBits = totalBits - prefixLen;

        BigInteger mask = (BigInteger.One << hostBits) - 1;
        BigInteger start = ipNum & ~mask;
        BigInteger end = start | mask;

        return (start, end);
    }

    static BigInteger ParseIp(string ip)
    {
        var addr = IPAddress.Parse(ip);
        var bytes = addr.GetAddressBytes();

        if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
        }
        else
        {
            Array.Reverse(bytes);
            return new BigInteger(bytes);
        }
    }

    static void Main()
    {
        var csvPath = Path.Combine(AppContext.BaseDirectory, "geo-US_utf8.csv");
        LoadDatabase(csvPath);
    }
}
