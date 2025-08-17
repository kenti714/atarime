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
    private readonly Button _fetchAllButton;
    private readonly TextBox _output;

    public MainForm()
    {
        Text = "Atarime";
        Width = 600;
        Height = 400;

        _fetchButton = new Button { Text = "直近の結果を取得", AutoSize = true };
        _showButton = new Button { Text = "直近の抽選結果を表示", AutoSize = true };
        _fetchAllButton = new Button { Text = "過去の抽選結果をすべて取得", AutoSize = true };
        _fetchButton.Click += async (s, e) => await FetchLatestAsync();
        _showButton.Click += (s, e) => DisplayLatest();
        _fetchAllButton.Click += async (s, e) => await FetchAllAsync();

        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        panel.Controls.AddRange(new Control[] { _fetchButton, _showButton, _fetchAllButton });

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
        void Log(string message)
        {
            if (_output.InvokeRequired)
                _output.Invoke(new Action(() => _output.AppendText(message + Environment.NewLine)));
            else
                _output.AppendText(message + Environment.NewLine);
        }

        _output.AppendText("Fetching latest LOTO6..." + Environment.NewLine);
        var loto6 = await LotoFetcher.FetchLoto6Async(Log);
        if (loto6 != null)
        {
            _output.AppendText($"No.{loto6.No} {loto6.Date:yyyyMMdd}: {string.Join(",", loto6.Numbers)} +[{loto6.Bonus}]" + Environment.NewLine);

            CsvStorage.AppendLoto6("loto6result.csv", loto6);
            _output.AppendText("Saved LOTO6 result." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("Failed to fetch LOTO6." + Environment.NewLine);
        }

        _output.AppendText("Fetching latest LOTO7..." + Environment.NewLine);
        var loto7 = await LotoFetcher.FetchLoto7Async(Log);
        if (loto7 != null)
        {
            _output.AppendText($"No.{loto7.No} {loto7.Date:yyyyMMdd}: {string.Join(",", loto7.Numbers)} +[{string.Join(",", loto7.Bonus)}]" + Environment.NewLine);

            CsvStorage.AppendLoto7("loto7result.csv", loto7);
            _output.AppendText("Saved LOTO7 result." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("Failed to fetch LOTO7." + Environment.NewLine);
        }
    }

    private async Task FetchAllAsync()
    {
        void Log(string message)
        {
            if (_output.InvokeRequired)
                _output.Invoke(new Action(() => _output.AppendText(message + Environment.NewLine)));
            else
                _output.AppendText(message + Environment.NewLine);
        }

        _output.AppendText("Fetching all LOTO6..." + Environment.NewLine);
        var all6 = await LotoFetcher.FetchAllLoto6Async(Log);
        CsvStorage.WriteLoto6("loto6result.csv", all6);
        _output.AppendText($"Saved {all6.Count} LOTO6 results." + Environment.NewLine);

        _output.AppendText("Fetching all LOTO7..." + Environment.NewLine);
        var all7 = await LotoFetcher.FetchAllLoto7Async(Log);
        CsvStorage.WriteLoto7("loto7result.csv", all7);
        _output.AppendText($"Saved {all7.Count} LOTO7 results." + Environment.NewLine);
    }

    private void DisplayLatest()
    {
        var last6 = CsvStorage.ReadLoto6("loto6result.csv").LastOrDefault();
        if (last6 != null)
            _output.AppendText($"Latest LOTO6: No.{last6.No} {last6.Date:yyyyMMdd} {string.Join(",", last6.Numbers)} +[{last6.Bonus}]{Environment.NewLine}");
        else
            _output.AppendText("Latest LOTO6: no data\n");

        var last7 = CsvStorage.ReadLoto7("loto7result.csv").LastOrDefault();
        if (last7 != null)
            _output.AppendText($"Latest LOTO7: No.{last7.No} {last7.Date:yyyyMMdd} {string.Join(",", last7.Numbers)} +[{string.Join(",", last7.Bonus)}]{Environment.NewLine}");
        else
            _output.AppendText("Latest LOTO7: no data\n");
    }
}
