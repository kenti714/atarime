using System;
using System.Windows.Forms;

namespace Atarime.Winform;

internal static class Program
{
    // Entry point for the main menu application.
    // Displays a window allowing navigation to various tools.
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
