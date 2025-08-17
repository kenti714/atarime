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
        _fetchAllButton = new Button { Text = "過去の抽選結果をすべて取得", AutoSize = true };
        _showButton = new Button { Text = "直近の抽選結果を表示", AutoSize = true };
        _fetchButton.Click += async (s, e) => await FetchLatestAsync();
        _showButton.Click += (s, e) => DisplayLatest();
        _fetchAllButton.Click += async (s, e) => await FetchAllAsync();

        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        panel.Controls.AddRange(new Control[] { _fetchButton, _fetchAllButton , _showButton });

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
        var existing6 = CsvStorage.ReadLoto6(ResultPaths.Loto6);
        int next6 = existing6.Count > 0 ? existing6.Max(r => r.No) + 1 : 1;
        var new6 = await LotoFetcher.FetchLoto6SinceAsync(next6, Log);
        if (new6.Count > 0)
        {
            foreach (var r in new6)
            {
                _output.AppendText($"No.{r.No} {r.Date:yyyyMMdd}: {string.Join(",", r.Numbers)} +[{r.Bonus}]" + Environment.NewLine);
                CsvStorage.AppendLoto6(ResultPaths.Loto6, r);
            }
            _output.AppendText($"Saved {new6.Count} LOTO6 result(s)." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("No new LOTO6 result." + Environment.NewLine);
        }

        _output.AppendText("Fetching latest LOTO7..." + Environment.NewLine);
        var existing7 = CsvStorage.ReadLoto7(ResultPaths.Loto7);
        int next7 = existing7.Count > 0 ? existing7.Max(r => r.No) + 1 : 1;
        var new7 = await LotoFetcher.FetchLoto7SinceAsync(next7, Log);
        if (new7.Count > 0)
        {
            foreach (var r in new7)
            {
                _output.AppendText($"No.{r.No} {r.Date:yyyyMMdd}: {string.Join(",", r.Numbers)} +[{string.Join(",", r.Bonus)}]" + Environment.NewLine);
                CsvStorage.AppendLoto7(ResultPaths.Loto7, r);
            }
            _output.AppendText($"Saved {new7.Count} LOTO7 result(s)." + Environment.NewLine);
        }
        else
        {
            _output.AppendText("No new LOTO7 result." + Environment.NewLine);
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
        var list6 = CsvStorage.ReadLoto6(ResultPaths.Loto6);
        int next6 = list6.Count > 0 ? list6.Max(r => r.No) + 1 : 1;
        var more6 = await LotoFetcher.FetchLoto6SinceAsync(next6, Log);
        if (more6.Count > 0)
        {
            list6.AddRange(more6);
            CsvStorage.WriteLoto6(ResultPaths.Loto6, list6);
        }
        _output.AppendText($"Saved {more6.Count} new LOTO6 results." + Environment.NewLine);

        _output.AppendText("Fetching all LOTO7..." + Environment.NewLine);
        var list7 = CsvStorage.ReadLoto7(ResultPaths.Loto7);
        int next7 = list7.Count > 0 ? list7.Max(r => r.No) + 1 : 1;
        var more7 = await LotoFetcher.FetchLoto7SinceAsync(next7, Log);
        if (more7.Count > 0)
        {
            list7.AddRange(more7);
            CsvStorage.WriteLoto7(ResultPaths.Loto7, list7);
        }
        _output.AppendText($"Saved {more7.Count} new LOTO7 results." + Environment.NewLine);
    }

    private void DisplayLatest()
    {
        var last6 = CsvStorage.ReadLoto6(ResultPaths.Loto6).LastOrDefault();
        if (last6 != null)
            _output.AppendText($"Latest LOTO6: No.{last6.No} {last6.Date:yyyyMMdd} {string.Join(",", last6.Numbers)} +[{last6.Bonus}]{Environment.NewLine}");
        else
            _output.AppendText("Latest LOTO6: no data\n");

        var last7 = CsvStorage.ReadLoto7(ResultPaths.Loto7).LastOrDefault();
        if (last7 != null)
            _output.AppendText($"Latest LOTO7: No.{last7.No} {last7.Date:yyyyMMdd} {string.Join(",", last7.Numbers)} +[{string.Join(",", last7.Bonus)}]{Environment.NewLine}");
        else
            _output.AppendText("Latest LOTO7: no data\n");
    }
}
