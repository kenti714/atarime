using System;

namespace Atarime.Core;

public record Loto6Result(DateTime Date, int[] Numbers, int Bonus);

public record Loto7Result(DateTime Date, int[] Numbers, int[] Bonus);
