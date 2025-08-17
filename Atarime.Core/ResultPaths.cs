using System;
using System.IO;

namespace Atarime.Core;

/// <summary>
/// Provides common file paths for CSV outputs.
/// </summary>
public static class ResultPaths
{
    /// <summary>Directory where CSV results are stored.</summary>
    public static string ResultDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "result");

    static ResultPaths()
    {
        Directory.CreateDirectory(ResultDirectory);
    }

    /// <summary>Path to LOTO6 result CSV.</summary>
    public static string Loto6 => Path.Combine(ResultDirectory, "loto6result.csv");

    /// <summary>Path to LOTO7 result CSV.</summary>
    public static string Loto7 => Path.Combine(ResultDirectory, "loto7result.csv");

    /// <summary>Path to user selected LOTO6 numbers.</summary>
    public static string MySelect6 => Path.Combine(ResultDirectory, "myselect6.csv");

    /// <summary>Path to user selected LOTO7 numbers.</summary>
    public static string MySelect7 => Path.Combine(ResultDirectory, "myselect7.csv");
}

