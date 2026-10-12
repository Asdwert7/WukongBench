using System.Runtime.InteropServices;

namespace WukongBench.Services;

public static class WindowHelper
{
    private const int Minimize = 6;
    private const int Restore = 9;

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr window, int command);

    public static void MinimizeConsole()
    {
        ShowWindow(GetConsoleWindow(), Minimize);
    }

    public static void RestoreConsole()
    {
        ShowWindow(GetConsoleWindow(), Restore);
    }
}