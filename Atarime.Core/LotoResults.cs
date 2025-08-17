using System;

namespace Atarime.Core;

public record Loto6Result(int No, DateTime Date, int[] Numbers, int Bonus);

public record Loto7Result(int No, DateTime Date, int[] Numbers, int[] Bonus);
