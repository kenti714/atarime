using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Atarime.Core;

namespace Atarime.MySelect;

public class MainForm : Form
{
    private readonly ListBox _loto6List;
    private readonly ListBox _loto7List;
    private readonly Button _save6Button;
    private readonly Button _save7Button;

    public MainForm()
    {
        Text = "My Select";
        Width = 600;
        Height = 400;

        _loto6List = new ListBox { SelectionMode = SelectionMode.MultiSimple, Dock = DockStyle.Fill };
        for (int i = 1; i <= 43; i++) _loto6List.Items.Add(i.ToString("00"));
        _save6Button = new Button { Text = "Save LOTO6", Dock = DockStyle.Bottom };
        _save6Button.Click += (s, e) => SaveSelection(_loto6List, ResultPaths.MySelect6, 6);

        _loto7List = new ListBox { SelectionMode = SelectionMode.MultiSimple, Dock = DockStyle.Fill };
        for (int i = 1; i <= 37; i++) _loto7List.Items.Add(i.ToString("00"));
        _save7Button = new Button { Text = "Save LOTO7", Dock = DockStyle.Bottom };
        _save7Button.Click += (s, e) => SaveSelection(_loto7List, ResultPaths.MySelect7, 7);

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(_loto6List, 0, 0);
        panel.Controls.Add(_loto7List, 1, 0);
        panel.Controls.Add(_save6Button, 0, 1);
        panel.Controls.Add(_save7Button, 1, 1);

        Controls.Add(panel);
    }

    private void SaveSelection(ListBox list, string path, int count)
    {
        if (list.SelectedItems.Count != count)
        {
            MessageBox.Show($"Please select {count} numbers.");
            return;
        }
        var line = string.Join(",", list.SelectedItems.Cast<string>());
        File.AppendAllText(path, line + Environment.NewLine);
        MessageBox.Show("Saved.");
    }
}

