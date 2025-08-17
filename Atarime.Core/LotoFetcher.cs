using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace Atarime.Core;

public static class LotoFetcher
{
    private static readonly HttpClient _http = new();

    static LotoFetcher()
    {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static async Task<Loto6Result?> FetchLoto6Async(Action<string>? log = null)
    {
        try
        {
            var nameUrl = "https://www.mizuhobank.co.jp/takarakuji/apl/txt/loto6/name.txt";
            log?.Invoke($"GET {nameUrl}");
            var nameResp = await _http.GetAsync(nameUrl);
            log?.Invoke($"Status {(int)nameResp.StatusCode} {nameResp.ReasonPhrase}");
            var nameTxt = await nameResp.Content.ReadAsStringAsync();
            log?.Invoke(nameTxt.Length > 200 ? nameTxt.Substring(0,200) + "..." : nameTxt);
            nameResp.EnsureSuccessStatusCode();

            var fileMatch = Regex.Match(nameTxt, @"A\d+\.CSV");
            if (!fileMatch.Success)
                throw new Exception("CSV name not found");

            var csvUrl = $"https://www.mizuhobank.co.jp/retail/takarakuji/loto/loto6/csv/{fileMatch.Value}";
            log?.Invoke($"GET {csvUrl}");
            var csvResp = await _http.GetAsync(csvUrl);
            log?.Invoke($"Status {(int)csvResp.StatusCode} {csvResp.ReasonPhrase}");
            csvResp.EnsureSuccessStatusCode();
            var bytes = await csvResp.Content.ReadAsByteArrayAsync();
            var csv = System.Text.Encoding.GetEncoding("shift_jis").GetString(bytes);
            log?.Invoke(csv.Length > 200 ? csv.Substring(0,200) + "..." : csv);

            var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3)
                throw new Exception("CSV format unexpected");

            var header = lines[1].Split(',');
            var nums = lines[3].Split(',');

            var date = ParseJapaneseDate(header[2]);
            var numbers = nums.Skip(1).Take(6).Select(s => int.Parse(s)).ToArray();
            var bonus = int.Parse(nums[^1]);

            return new Loto6Result(date, numbers, bonus);
        }
        catch (Exception ex)
        {
            log?.Invoke($"Fetch LOTO6 failed: {ex.Message}");
            if (log == null)
                Console.Error.WriteLine($"Fetch LOTO6 failed: {ex.Message}");
        }

        return null;
    }

    private static DateTime ParseJapaneseDate(string s)
    {
        var m = Regex.Match(s, @"令和(\d+)年(\d+)月(\d+)日");
        if (m.Success)
        {
            int year = int.Parse(m.Groups[1].Value) + 2018;
            int month = int.Parse(m.Groups[2].Value);
            int day = int.Parse(m.Groups[3].Value);
            return new DateTime(year, month, day);
        }
        return DateTime.Parse(s, CultureInfo.InvariantCulture);
    }

    public static async Task<Loto7Result?> FetchLoto7Async(Action<string>? log = null)
    {
        try
        {
            var url = "https://www.mizuhobank.co.jp/takarakuji/check/loto/loto7/index.html";
            log?.Invoke($"GET {url}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _http.SendAsync(request);
            log?.Invoke($"Status {(int)response.StatusCode} {response.ReasonPhrase}");
            var html = await response.Content.ReadAsStringAsync();
            log?.Invoke(html.Length > 200 ? html.Substring(0,200) + "..." : html);
            response.EnsureSuccessStatusCode();

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
            log?.Invoke($"Fetch LOTO7 failed: {ex.Message}");
            if (log == null)
                Console.Error.WriteLine($"Fetch LOTO7 failed: {ex.Message}");
        }
        return null;
    }
}
