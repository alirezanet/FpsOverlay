using System.Runtime.InteropServices;

namespace FpsOverlay.Lib.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/uxtheme/ns-uxtheme-_margins
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class Margins
    {
        public int Left, Right, Top, Bottom;
    }
}
