using System.Collections.Generic;
using System.Linq;
using Atarime.Core;

namespace Atarime.Statistics;

public static class StatisticsCalculator
{
    private static Dictionary<int, int> CountNumbers(IEnumerable<IEnumerable<int>> sequences)
    {
        var dict = new Dictionary<int, int>();
        foreach (var seq in sequences)
        {
            foreach (var n in seq)
            {
                dict[n] = dict.TryGetValue(n, out var c) ? c + 1 : 1;
            }
        }
        return dict;
    }

    public static Dictionary<int, int> CalculateNumberFrequencies(IEnumerable<Loto6Result> results)
        => CountNumbers(results.Select(r => r.Numbers));

    public static Dictionary<int, int> CalculateNumberFrequencies(IEnumerable<Loto7Result> results)
        => CountNumbers(results.Select(r => r.Numbers));

    private static IEnumerable<KeyValuePair<int, int>> TopNumbers(Dictionary<int, int> frequencies, int topCount)
        => frequencies
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Take(topCount);

    public static IEnumerable<KeyValuePair<int, int>> GetMostFrequentNumbers(
        IEnumerable<Loto6Result> results, int windowSize, int topCount)
    {
        var subset = results.TakeLast(windowSize);
        var freq = CalculateNumberFrequencies(subset);
        return TopNumbers(freq, topCount);
    }

    public static IEnumerable<KeyValuePair<int, int>> GetMostFrequentNumbers(
        IEnumerable<Loto7Result> results, int windowSize, int topCount)
    {
        var subset = results.TakeLast(windowSize);
        var freq = CalculateNumberFrequencies(subset);
        return TopNumbers(freq, topCount);
    }
}
