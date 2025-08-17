using System;
using System.Collections.Generic;
using System.IO;

namespace Atarime.Core;

public class ConfigManager
{
    private readonly Dictionary<string, string> _values = new();

    public string? Get(string key) => _values.TryGetValue(key, out var v) ? v : null;

    public void Set(string key, string value) => _values[key] = value;

    public static ConfigManager Load(string path)
    {
        var cfg = new ConfigManager();
        if (!File.Exists(path)) return cfg;
        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || !trimmed.Contains('='))
                continue;
            var parts = trimmed.Split('=', 2);
            cfg._values[parts[0].Trim()] = parts[1].Trim();
        }
        return cfg;
    }

    public void Save(string path)
    {
        var lines = new List<string>();
        foreach (var kv in _values)
        {
            lines.Add($"{kv.Key}={kv.Value}");
        }
        File.WriteAllLines(path, lines);
    }
}
