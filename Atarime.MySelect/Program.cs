using System;
using System.Windows.Forms;

namespace Atarime.MySelect;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

