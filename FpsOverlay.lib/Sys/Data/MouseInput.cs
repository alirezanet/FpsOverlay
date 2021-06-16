using System;
using System.Runtime.InteropServices;

namespace FpsOverlay.Lib.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
