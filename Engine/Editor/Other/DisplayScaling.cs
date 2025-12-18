using System.Runtime.InteropServices;

namespace Concrete;

public static class DisplayScaling
{
    [DllImport("User32.dll")]
    private static extern uint GetDpiForSystem();

    public static float displayScale = GetSystemDisplayScale();

    public static float GetSystemDisplayScale()
    {
        if (OperatingSystem.IsWindows())
        {
            uint dpi = GetDpiForSystem();
            float scale = dpi / 96f;
            return scale;
        }
        else
        {
            return 1.0f;
        }
    }
}