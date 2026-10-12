using System.Runtime.InteropServices;

namespace WukongBench.Services;

public static class Mouse
{
    private const uint MoveFlag = 0x0001;
    private const uint LeftDown = 0x0002;
    private const uint LeftUp = 0x0004;
    private const uint Absolute = 0x8000;

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extraInfo);

    public static void Click(double x, double y)
    {
        uint dx = (uint)(x * 65535);
        uint dy = (uint)(y * 65535);

        mouse_event(MoveFlag | Absolute, dx, dy, 0, UIntPtr.Zero);
        Thread.Sleep(300);

        mouse_event(LeftDown, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(80);
        mouse_event(LeftUp, 0, 0, 0, UIntPtr.Zero);
    }
}