using System.Drawing;
using System.Runtime.InteropServices;

namespace WukongBench.Services;

public static class ScreenCapture
{
    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    // Нашел что при 125%/150% масштабе размер экрана сьедет
    public static void EnableDpiAwareness()
    {
        SetProcessDPIAware();
    }
    public static int GetWidth()
    {
        return GetSystemMetrics(0);
    }

    public static int GetHeight()
    {
        return GetSystemMetrics(1);
    }

    public static Bitmap Capture()
    {
        Bitmap bitmap = new Bitmap(GetWidth(), GetHeight());
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
        return bitmap;
    }
}