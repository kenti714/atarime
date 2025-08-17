using System.Collections.Generic;
using System.Linq;

namespace Atarime.AI;

public static class PredictionEngine
{
    public static int[] PredictNext(Dictionary<int, int> frequencies, int count = 6)
    {
        return frequencies
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Take(count)
            .Select(kv => kv.Key)
            .ToArray();
    }
}
