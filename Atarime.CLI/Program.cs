using System;
using System.Threading.Tasks;
using Atarime.Core;

namespace Atarime.CLI;

internal class Program
{
    static async Task Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "loto6";
        var all = args.Length > 1 && args[1].ToLowerInvariant() == "all";

        if (mode == "loto7")
        {
            if (all)
            {
                Console.WriteLine("Fetching all LOTO7 results...");
                var results = await LotoFetcher.FetchAllLoto7Async(Console.WriteLine);
                CsvStorage.WriteLoto7("loto7result.csv", results);
                Console.WriteLine($"Saved {results.Count} LOTO7 results.");
            }
            else
            {
                Console.WriteLine("Fetching latest LOTO7 result...");
                var result = await LotoFetcher.FetchLoto7Async(Console.WriteLine);
                if (result != null)
                {
                    Console.WriteLine($"No.{result.No} {result.Date:yyyyMMdd}: {string.Join(",", result.Numbers)} +[{string.Join(",", result.Bonus)}]");
                    CsvStorage.AppendLoto7("loto7result.csv", result);
                    Console.WriteLine("Saved LOTO7 result.");
                }
                else
                {
                    Console.WriteLine("Failed to fetch LOTO7 result.");
                }
            }
        }
        else
        {
            if (all)
            {
                Console.WriteLine("Fetching all LOTO6 results...");
                var results = await LotoFetcher.FetchAllLoto6Async(Console.WriteLine);
                CsvStorage.WriteLoto6("loto6result.csv", results);
                Console.WriteLine($"Saved {results.Count} LOTO6 results.");
            }
            else
            {
                Console.WriteLine("Fetching latest LOTO6 result...");
                var result = await LotoFetcher.FetchLoto6Async(Console.WriteLine);
                if (result != null)
                {
                    Console.WriteLine($"No.{result.No} {result.Date:yyyyMMdd}: {string.Join(",", result.Numbers)} +[{result.Bonus}]");
                    CsvStorage.AppendLoto6("loto6result.csv", result);
                    Console.WriteLine("Saved LOTO6 result.");
                }
                else
                {
                    Console.WriteLine("Failed to fetch LOTO6 result.");
                }
            }
        }
    }
}
