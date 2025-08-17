using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Atarime.Core;

public static class CsvStorage
{
    private static readonly string[] Loto6Header =
        { "no", "date", "honsuji1", "honsuji2", "honsuji3", "honsuji4", "honsuji5", "honsuji6", "bonus_suji1" };

    private static readonly string[] Loto7Header =
        { "no", "date", "honsuji1", "honsuji2", "honsuji3", "honsuji4", "honsuji5", "honsuji6", "honsuji7", "bonus_suji1", "bonus_suji2" };

    public static void AppendLoto6(string path, Loto6Result result)
    {
        EnsureHeader(path, Loto6Header);
        var line = string.Join(",",
            $"第{result.No}回",
            result.Date.ToString("yyyyMMdd"),
            string.Join(",", result.Numbers.Select(n => n.ToString("00"))),
            result.Bonus.ToString("00"));
        File.AppendAllText(path, line + Environment.NewLine);
    }

    public static void AppendLoto7(string path, Loto7Result result)
    {
        EnsureHeader(path, Loto7Header);
        var line = string.Join(",",
            $"第{result.No}回",
            result.Date.ToString("yyyyMMdd"),
            string.Join(",", result.Numbers.Select(n => n.ToString("00"))),
            string.Join(",", result.Bonus.Select(n => n.ToString("00"))));
        File.AppendAllText(path, line + Environment.NewLine);
    }

    public static List<Loto6Result> ReadLoto6(string path)
    {
        var list = new List<Loto6Result>();
        if (!File.Exists(path)) return list;
        foreach (var line in File.ReadLines(path).Skip(1))
        {
            var parts = line.Split(',');
            if (parts.Length < 9) continue;
            var no = ParseNo(parts[0]);
            var date = DateTime.ParseExact(parts[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            var numbers = parts.Skip(2).Take(6).Select(int.Parse).ToArray();
            var bonus = int.Parse(parts[8]);
            list.Add(new Loto6Result(no, date, numbers, bonus));
        }
        return list;
    }

    public static List<Loto7Result> ReadLoto7(string path)
    {
        var list = new List<Loto7Result>();
        if (!File.Exists(path)) return list;
        foreach (var line in File.ReadLines(path).Skip(1))
        {
            var parts = line.Split(',');
            if (parts.Length < 11) continue;
            var no = ParseNo(parts[0]);
            var date = DateTime.ParseExact(parts[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            var numbers = parts.Skip(2).Take(7).Select(int.Parse).ToArray();
            var bonus = parts.Skip(9).Take(2).Select(int.Parse).ToArray();
            list.Add(new Loto7Result(no, date, numbers, bonus));
        }
        return list;
    }

    public static void WriteLoto6(string path, IEnumerable<Loto6Result> results)
    {
        var lines = new List<string> { string.Join(",", Loto6Header) };
        lines.AddRange(results.Select(r => string.Join(",",
            $"第{r.No}回",
            r.Date.ToString("yyyyMMdd"),
            string.Join(",", r.Numbers.Select(n => n.ToString("00"))),
            r.Bonus.ToString("00"))));
        File.WriteAllLines(path, lines);
    }

    public static void WriteLoto7(string path, IEnumerable<Loto7Result> results)
    {
        var lines = new List<string> { string.Join(",", Loto7Header) };
        lines.AddRange(results.Select(r => string.Join(",",
            $"第{r.No}回",
            r.Date.ToString("yyyyMMdd"),
            string.Join(",", r.Numbers.Select(n => n.ToString("00"))),
            string.Join(",", r.Bonus.Select(n => n.ToString("00"))))));
        File.WriteAllLines(path, lines);
    }

    private static void EnsureHeader(string path, string[] header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, string.Join(",", header) + Environment.NewLine);
        }
    }

    private static int ParseNo(string s)
    {
        var m = System.Text.RegularExpressions.Regex.Match(s, @"第(\d+)回");
        return m.Success ? int.Parse(m.Groups[1].Value) : int.Parse(s);
    }
}
