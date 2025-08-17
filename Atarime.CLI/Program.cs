using System;
using System.Threading.Tasks;
using Atarime.Core;

namespace Atarime.CLI;

internal class Program
{
    static async Task Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "loto6";
        if (mode == "loto7")
        {
            var result = await LotoFetcher.FetchLoto7Async();
            if (result != null)
            {
                CsvStorage.AppendLoto7("loto7result.csv", result);
                Console.WriteLine("Saved LOTO7 result.");
            }
        }
        else
        {
            var result = await LotoFetcher.FetchLoto6Async();
            if (result != null)
            {
                CsvStorage.AppendLoto6("loto6result.csv", result);
                Console.WriteLine("Saved LOTO6 result.");
            }
        }
    }
}
