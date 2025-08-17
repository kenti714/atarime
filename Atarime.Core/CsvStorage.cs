using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Atarime.Core;

public static class CsvStorage
{
    public static void AppendLoto6(string path, Loto6Result result)
    {
        var line = string.Join(",",
            result.Date.ToString("yyyyMMdd"),
            string.Join(",", result.Numbers.Select(n => n.ToString("00"))),
            result.Bonus.ToString("00"));
        File.AppendAllText(path, line + Environment.NewLine);
    }

    public static List<Loto6Result> ReadLoto6(string path)
    {
        var list = new List<Loto6Result>();
        if (!File.Exists(path)) return list;
        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length < 8) continue;
            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);
            var numbers = parts.Skip(1).Take(6).Select(int.Parse).ToArray();
            var bonus = int.Parse(parts[7]);
            list.Add(new Loto6Result(date, numbers, bonus));
        }
        return list;
    }

    public static void AppendLoto7(string path, Loto7Result result)
    {
        var line = string.Join(",",
            result.Date.ToString("yyyyMMdd"),
            string.Join(",", result.Numbers.Select(n => n.ToString("00"))),
            string.Join(",", result.Bonus.Select(n => n.ToString("00"))));
        File.AppendAllText(path, line + Environment.NewLine);
    }

    public static List<Loto7Result> ReadLoto7(string path)
    {
        var list = new List<Loto7Result>();
        if (!File.Exists(path)) return list;
        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length < 10) continue;
            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);
            var numbers = parts.Skip(1).Take(7).Select(int.Parse).ToArray();
            var bonus = parts.Skip(8).Take(2).Select(int.Parse).ToArray();
            list.Add(new Loto7Result(date, numbers, bonus));
        }
        return list;
    }
}
