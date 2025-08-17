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

            return await FetchLoto6ByFileAsync(fileMatch.Value, log);
        }
        catch (Exception ex)
        {
            log?.Invoke($"Fetch LOTO6 failed: {ex.Message}");
            if (log == null)
                Console.Error.WriteLine($"Fetch LOTO6 failed: {ex.Message}");
        }

        return null;
    }

    private static async Task<Loto6Result?> FetchLoto6ByFileAsync(string file, Action<string>? log)
    {
        var csvUrl = $"https://www.mizuhobank.co.jp/retail/takarakuji/loto/loto6/csv/{file}";
        log?.Invoke($"GET {csvUrl}");
        var csvResp = await _http.GetAsync(csvUrl);
        log?.Invoke($"Status {(int)csvResp.StatusCode} {csvResp.ReasonPhrase}");
        csvResp.EnsureSuccessStatusCode();
        var bytes = await csvResp.Content.ReadAsByteArrayAsync();
        var csv = Encoding.GetEncoding("shift_jis").GetString(bytes);
        log?.Invoke(csv.Length > 200 ? csv.Substring(0,200) + "..." : csv);

        var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 4)
            throw new Exception("CSV format unexpected");

        var header = lines[1].Split(',');
        var nums = lines[3].Split(',');

        var noMatch = Regex.Match(header[0], @"第(\d+)回");
        var no = noMatch.Success ? int.Parse(noMatch.Groups[1].Value) : 0;
        var date = ParseJapaneseDate(header[2]);
        var numbers = nums.Skip(1).Take(6).Select(s => int.Parse(s)).ToArray();
        var bonus = int.Parse(nums[^1]);

        return new Loto6Result(no, date, numbers, bonus);
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
            var nameUrl = "https://www.mizuhobank.co.jp/takarakuji/apl/txt/loto7/name.txt";
            log?.Invoke($"GET {nameUrl}");
            var nameResp = await _http.GetAsync(nameUrl);
            log?.Invoke($"Status {(int)nameResp.StatusCode} {nameResp.ReasonPhrase}");
            var nameTxt = await nameResp.Content.ReadAsStringAsync();
            log?.Invoke(nameTxt.Length > 200 ? nameTxt.Substring(0,200) + "..." : nameTxt);
            nameResp.EnsureSuccessStatusCode();

            var fileMatch = Regex.Match(nameTxt, @"A\d+\.CSV");
            if (!fileMatch.Success)
                throw new Exception("CSV name not found");

            return await FetchLoto7ByFileAsync(fileMatch.Value, log);
        }
        catch (Exception ex)
        {
            log?.Invoke($"Fetch LOTO7 failed: {ex.Message}");
            if (log == null)
                Console.Error.WriteLine($"Fetch LOTO7 failed: {ex.Message}");
        }
        return null;
    }

    private static async Task<Loto7Result?> FetchLoto7ByFileAsync(string file, Action<string>? log)
    {
        var csvUrl = $"https://www.mizuhobank.co.jp/retail/takarakuji/loto/loto7/csv/{file}";
        log?.Invoke($"GET {csvUrl}");
        var csvResp = await _http.GetAsync(csvUrl);
        log?.Invoke($"Status {(int)csvResp.StatusCode} {csvResp.ReasonPhrase}");
        csvResp.EnsureSuccessStatusCode();
        var bytes = await csvResp.Content.ReadAsByteArrayAsync();
        var csv = Encoding.GetEncoding("shift_jis").GetString(bytes);
        log?.Invoke(csv.Length > 200 ? csv.Substring(0,200) + "..." : csv);

        var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 4)
            throw new Exception("CSV format unexpected");

        var header = lines[1].Split(',');
        var nums = lines[3].Split(',');

        var noMatch = Regex.Match(header[0], @"第(\d+)回");
        var no = noMatch.Success ? int.Parse(noMatch.Groups[1].Value) : 0;
        var date = ParseJapaneseDate(header[2]);
        var numbers = nums.Skip(1).Take(7).Select(int.Parse).ToArray();
        var bonus = nums.Skip(9).Take(2).Select(int.Parse).ToArray();

        return new Loto7Result(no, date, numbers, bonus);
    }

    public static async Task<List<Loto6Result>> FetchAllLoto6Async(Action<string>? log = null)
    {
        var results = new List<Loto6Result>();
        try
        {
            var nameUrl = "https://www.mizuhobank.co.jp/takarakuji/apl/txt/loto6/name.txt";
            log?.Invoke($"GET {nameUrl}");
            var nameTxt = await _http.GetStringAsync(nameUrl);
            log?.Invoke(nameTxt.Length > 200 ? nameTxt.Substring(0,200) + "..." : nameTxt);
            var fileMatch = Regex.Match(nameTxt, @"A(\d{3})(\d{4})\.CSV");
            if (!fileMatch.Success) throw new Exception("CSV name not found");
            var prefix = fileMatch.Groups[1].Value;
            int latest = int.Parse(fileMatch.Groups[2].Value);
            for (int i = 1; i <= latest; i++)
            {
                var file = $"A{prefix}{i:0000}.CSV";
                try
                {
                    var r = await FetchLoto6ByFileAsync(file, log);
                    if (r != null) results.Add(r);
                }
                catch (Exception ex)
                {
                    log?.Invoke($"Skip {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            log?.Invoke($"FetchAll LOTO6 failed: {ex.Message}");
        }
        return results;
    }

    public static async Task<List<Loto7Result>> FetchAllLoto7Async(Action<string>? log = null)
    {
        var results = new List<Loto7Result>();
        try
        {
            var nameUrl = "https://www.mizuhobank.co.jp/takarakuji/apl/txt/loto7/name.txt";
            log?.Invoke($"GET {nameUrl}");
            var nameTxt = await _http.GetStringAsync(nameUrl);
            log?.Invoke(nameTxt.Length > 200 ? nameTxt.Substring(0,200) + "..." : nameTxt);
            var fileMatch = Regex.Match(nameTxt, @"A(\d{3})(\d{4})\.CSV");
            if (!fileMatch.Success) throw new Exception("CSV name not found");
            var prefix = fileMatch.Groups[1].Value;
            int latest = int.Parse(fileMatch.Groups[2].Value);
            for (int i = 1; i <= latest; i++)
            {
                var file = $"A{prefix}{i:0000}.CSV";
                try
                {
                    var r = await FetchLoto7ByFileAsync(file, log);
                    if (r != null) results.Add(r);
                }
                catch (Exception ex)
                {
                    log?.Invoke($"Skip {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            log?.Invoke($"FetchAll LOTO7 failed: {ex.Message}");
        }
        return results;
    }
}
