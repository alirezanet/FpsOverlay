using System.Runtime.InteropServices;

namespace FpsOverlay.Lib.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
    }
}
