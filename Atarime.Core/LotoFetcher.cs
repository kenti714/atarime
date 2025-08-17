using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Atarime.Core;

public static class LotoFetcher
{
    private static readonly HttpClient _http = new();

    static LotoFetcher()
    {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
    }

    public static async Task<Loto6Result?> FetchLoto6Async()
    {
        try
        {
            // The LOTO6 result is provided via JSON which is loaded by the web page.
            var json = await _http.GetStringAsync("https://www.mizuhobank.co.jp/takarakuji/lottery/json/loto6.json");

            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            JsonElement first = default;

            if (root.ValueKind == JsonValueKind.Array)
            {
                first = root.EnumerateArray().FirstOrDefault();
            }
            else
            {
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array &&
                        prop.Value.GetArrayLength() > 0 &&
                        prop.Value[0].ValueKind == JsonValueKind.Object)
                    {
                        first = prop.Value[0];
                        break;
                    }
                }
            }

            if (first.ValueKind == JsonValueKind.Object)
            {
                DateTime? date = null;
                int[]? numbers = null;
                int? bonus = null;

                foreach (var p in first.EnumerateObject())
                {
                    if (!date.HasValue && p.Name.Contains("date", StringComparison.OrdinalIgnoreCase) &&
                        p.Value.ValueKind == JsonValueKind.String)
                    {
                        var s = p.Value.GetString() ?? string.Empty;
                        s = s.Replace("年", "/").Replace("月", "/").Replace("日", "");
                        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                            date = d;
                    }
                    else if (p.Value.ValueKind == JsonValueKind.Array)
                    {
                        var arr = p.Value.EnumerateArray()
                                         .Where(e => e.ValueKind == JsonValueKind.Number)
                                         .Select(e => e.GetInt32())
                                         .ToArray();
                        if (arr.Length >= 6 && numbers == null)
                        {
                            numbers = arr.Take(6).ToArray();
                            if (arr.Length >= 7) bonus ??= arr[6];
                        }
                        else if (arr.Length == 1 && bonus == null)
                        {
                            bonus = arr[0];
                        }
                    }
                    else if (p.Value.ValueKind == JsonValueKind.Number &&
                             p.Name.Contains("bonus", StringComparison.OrdinalIgnoreCase) &&
                             !bonus.HasValue)
                    {
                        bonus = p.Value.GetInt32();
                    }
                }

                if (date.HasValue && numbers != null && bonus.HasValue)
                {
                    return new Loto6Result(date.Value, numbers, bonus.Value);
                }
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
