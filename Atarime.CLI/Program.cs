using System;
using System.Threading.Tasks;
using System.Linq;
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
            var existing = CsvStorage.ReadLoto7(ResultPaths.Loto7);
            int nextNo = existing.Count > 0 ? existing.Max(r => r.No) + 1 : 1;
            if (all)
            {
                Console.WriteLine("Fetching missing LOTO7 results...");
                var results = await LotoFetcher.FetchLoto7SinceAsync(nextNo, Console.WriteLine);
                if (results.Count > 0)
                {
                    existing.AddRange(results);
                    CsvStorage.WriteLoto7(ResultPaths.Loto7, existing);
                    Console.WriteLine($"Saved {results.Count} new LOTO7 results.");
                }
                else
                {
                    Console.WriteLine("No new LOTO7 results.");
                }
            }
            else
            {
                Console.WriteLine("Fetching latest LOTO7 result...");
                var results = await LotoFetcher.FetchLoto7SinceAsync(nextNo, Console.WriteLine);
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        Console.WriteLine($"No.{r.No} {r.Date:yyyyMMdd}: {string.Join(",", r.Numbers)} +[{string.Join(",", r.Bonus)}]");
                        CsvStorage.AppendLoto7(ResultPaths.Loto7, r);
                    }
                    Console.WriteLine($"Saved {results.Count} LOTO7 result(s).");
                }
                else
                {
                    Console.WriteLine("No new LOTO7 result.");
                }

            }
        }
        else
        {
            var existing = CsvStorage.ReadLoto6(ResultPaths.Loto6);
            int nextNo = existing.Count > 0 ? existing.Max(r => r.No) + 1 : 1;
            if (all)
            {
                Console.WriteLine("Fetching missing LOTO6 results...");
                var results = await LotoFetcher.FetchLoto6SinceAsync(nextNo, Console.WriteLine);
                if (results.Count > 0)
                {
                    existing.AddRange(results);
                    CsvStorage.WriteLoto6(ResultPaths.Loto6, existing);
                    Console.WriteLine($"Saved {results.Count} new LOTO6 results.");
                }
                else
                {
                    Console.WriteLine("No new LOTO6 results.");
                }
            }
            else
            {
                Console.WriteLine("Fetching latest LOTO6 result...");
                var results = await LotoFetcher.FetchLoto6SinceAsync(nextNo, Console.WriteLine);
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        Console.WriteLine($"No.{r.No} {r.Date:yyyyMMdd}: {string.Join(",", r.Numbers)} +[{r.Bonus}]");
                        CsvStorage.AppendLoto6(ResultPaths.Loto6, r);
                    }
                    Console.WriteLine($"Saved {results.Count} LOTO6 result(s).");
                }
                else
                {
                    Console.WriteLine("No new LOTO6 result.");
                }

            }
        }
    }
}
