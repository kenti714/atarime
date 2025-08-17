using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atarime.Core;

public static class LotoFetcher
{
    private static readonly HttpClient _http = new();

    public static async Task<Loto6Result?> FetchLoto6Async()
    {
        try
        {
            var html = await _http.GetStringAsync("https://www.mizuhobank.co.jp/takarakuji/check/loto/loto6/index.html");
            var numbers = Regex.Matches(html, @"<td class=""alnCenter"">(\d{1,2})</td>")
                               .Select(m => int.Parse(m.Groups[1].Value))
                               .ToList();
            if (numbers.Count >= 7)
            {
                var date = DateTime.Today;
                var main = numbers.Take(6).ToArray();
                var bonus = numbers[6];
                return new Loto6Result(date, main, bonus);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fetch LOTO6 failed: {ex.Message}");
        }
        return null;
    }

    public static async Task<Loto7Result?> FetchLoto7Async()
    {
        try
        {
            var html = await _http.GetStringAsync("https://www.mizuhobank.co.jp/takarakuji/check/loto/loto7/index.html");
            var numbers = Regex.Matches(html, @"<td class=""alnCenter"">(\d{1,2})</td>")
                               .Select(m => int.Parse(m.Groups[1].Value))
                               .ToList();
            if (numbers.Count >= 9)
            {
                var date = DateTime.Today;
                var main = numbers.Take(7).ToArray();
                var bonus = numbers.Skip(7).Take(2).ToArray();
                return new Loto7Result(date, main, bonus);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fetch LOTO7 failed: {ex.Message}");
        }
        return null;
    }
}
