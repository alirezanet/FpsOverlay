using System;

namespace FpsOverlay.Lib.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-hookproc
    /// </summary>
    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
}
