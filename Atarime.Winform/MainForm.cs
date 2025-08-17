using System;
using System.IO;
using System.Windows.Forms;
using Atarime.GetResltWinform;
using Atarime.Statistics;

namespace Atarime.Winform;

public class MainForm : Form
{
    private readonly Button _importButton;
    private readonly Button _statisticsButton;
    private readonly Button _selectButton;
    private readonly Button _exitButton;

    public MainForm()
    {
        Text = "Atarime";
        Width = 400;
        Height = 200;

        _importButton = new Button { Text = "過去の抽選結果を取り込み", AutoSize = true };
        _statisticsButton = new Button { Text = "過去の抽選結果から統計を調べる", AutoSize = true };
        _selectButton = new Button { Text = "自分の好きな数字を選ぶ", AutoSize = true };
        _exitButton = new Button { Text = "終了する", AutoSize = true };

        _importButton.Click += (s, e) => new GetResltWinForm().ShowDialog();
        _statisticsButton.Click += (s, e) => ShowStatistics();
        _selectButton.Click += (s, e) => new Atarime.MySelect.MainForm().ShowDialog();
        _exitButton.Click += (s, e) => Close();

        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        panel.Controls.AddRange(new Control[] { _importButton, _statisticsButton, _selectButton, _exitButton });
        Controls.Add(panel);
    }

    private void ShowStatistics()
    {
        using var writer = new StringWriter();
        var previous = Console.Out;
        Console.SetOut(writer);
        StatisticsPrinter.PrintLoto6();
        StatisticsPrinter.PrintLoto7();
        Console.SetOut(previous);
        MessageBox.Show(writer.ToString(), "Statistics");
    }
}
