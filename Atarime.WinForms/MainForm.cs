using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Atarime.Core;
using Atarime.Statistics;
using Atarime.AI;

namespace Atarime.WinForms;

public class MainForm : Form
{
    private readonly Button _loadButton;
    private readonly DataGridView _grid;

    public MainForm()
    {
        Text = "Atarime";
        _loadButton = new Button { Text = "Load LOTO6", Dock = DockStyle.Top };
        _loadButton.Click += LoadButton_Click;
        _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
        Controls.Add(_grid);
        Controls.Add(_loadButton);
    }

    private void LoadButton_Click(object? sender, EventArgs e)
    {
        const string path = "loto6result.csv";
        if (!File.Exists(path))
        {
            MessageBox.Show($"CSV not found: {path}");
            return;
        }

        var results = CsvStorage.ReadLoto6(path);
        var freq = StatisticsCalculator.CalculateNumberFrequencies(results);
        var prediction = PredictionEngine.PredictNext(freq);
        var display = freq
            .OrderByDescending(kv => kv.Value)
            .Select(kv => new { Number = kv.Key, Count = kv.Value, Predicted = prediction.Contains(kv.Key) })
            .ToList();
        _grid.DataSource = display;
    }
}
