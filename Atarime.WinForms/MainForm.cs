using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atarime.Core;

namespace Atarime.WinForms;

public class MainForm : Form
{
    private readonly Button _fetchButton;
    private readonly Button _showButton;
    private readonly TextBox _output;

    public MainForm()
    {
        Text = "Atarime";
        Width = 600;
        Height = 400;

        _fetchButton = new Button { Text = "直近の結果を取得", AutoSize = true };
        _showButton = new Button { Text = "直近の抽選結果を表示", AutoSize = true };
        _fetchButton.Click += async (s, e) => await FetchLatestAsync();
        _showButton.Click += (s, e) => DisplayLatest();

        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        panel.Controls.AddRange(new Control[] { _fetchButton, _showButton });

        _output = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        Controls.Add(_output);
        Controls.Add(panel);
    }

    private async Task FetchLatestAsync()
    {
        _output.AppendText("Fetching latest LOTO6..." + Environment.NewLine);
        var loto6 = await LotoFetcher.FetchLoto6Async();
        if (loto6 != null)
        {
            _output.AppendText($"Draw {loto6.Date:yyyyMMdd}: {string.Join(",", loto6.Numbers)} +[{loto6.Bonus}]" + Environment.NewLine);
            CsvStorage.AppendLoto6("loto6result.csv", loto6);
            _output.AppendText("Saved LOTO6 result." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("Failed to fetch LOTO6." + Environment.NewLine);
        }

        _output.AppendText("Fetching latest LOTO7..." + Environment.NewLine);
        var loto7 = await LotoFetcher.FetchLoto7Async();
        if (loto7 != null)
        {
            _output.AppendText($"Draw {loto7.Date:yyyyMMdd}: {string.Join(",", loto7.Numbers)} +[{string.Join(",", loto7.Bonus)}]" + Environment.NewLine);
            CsvStorage.AppendLoto7("loto7result.csv", loto7);
            _output.AppendText("Saved LOTO7 result." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("Failed to fetch LOTO7." + Environment.NewLine);
        }
    }

    private void DisplayLatest()
    {
        var last6 = CsvStorage.ReadLoto6("loto6result.csv").LastOrDefault();
        if (last6 != null)
        {
            _output.AppendText($"Latest LOTO6: {last6.Date:yyyyMMdd} {string.Join(",", last6.Numbers)} +[{last6.Bonus}]{Environment.NewLine}");
        }
        else
        {
            _output.AppendText("Latest LOTO6: no data\n");
        }

        var last7 = CsvStorage.ReadLoto7("loto7result.csv").LastOrDefault();
        if (last7 != null)
        {
            _output.AppendText($"Latest LOTO7: {last7.Date:yyyyMMdd} {string.Join(",", last7.Numbers)} +[{string.Join(",", last7.Bonus)}]{Environment.NewLine}");
        }
        else
        {
            _output.AppendText("Latest LOTO7: no data\n");
        }
    }
}
