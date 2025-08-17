using System;
using System.Linq;
using Atarime.Core;

namespace Atarime.Statistics;

/// <summary>
/// Helper class to print frequent numbers for recent LOTO6 and LOTO7 draws.
/// </summary>
public static class StatisticsPrinter
{
    private static readonly int[] Windows = { 100, 50, 25, 10, 5 };

    public static void PrintLoto6()
    {
        var results = CsvStorage.ReadLoto6(ResultPaths.Loto6);
        foreach (var w in Windows)
        {
            var top = StatisticsCalculator
                .GetMostFrequentNumbers(results, w, 6)
                .Select(kv => $"{kv.Key}({kv.Value})");
            Console.WriteLine($"LOTO6 last {Math.Min(w, results.Count)}: {string.Join(", ", top)}");
        }
    }

    public static void PrintLoto7()
    {
        var results = CsvStorage.ReadLoto7(ResultPaths.Loto7);
        foreach (var w in Windows)
        {
            var top = StatisticsCalculator
                .GetMostFrequentNumbers(results, w, 7)
                .Select(kv => $"{kv.Key}({kv.Value})");
            Console.WriteLine($"LOTO7 last {Math.Min(w, results.Count)}: {string.Join(", ", top)}");
        }
    }
}

