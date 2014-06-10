using System;
using System.Drawing;
using System.Runtime.InteropServices;
//using Point=System.Windows.Point;

namespace Ulavali
{
    public static class NativeMethods
    {
        internal const int GWL_EXSTYLE = -20;

        internal const int SW_SHOWNA = 8;
        internal const int SW_HIDE = 0;
        internal const int SWP_NOACTIVATE = 0x0010;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;
        internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(
            IntPtr hWnd, IntPtr hwndAfter, int x, int y,
            int width, int height, int flags);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex,
                                                 int dwNewLong);

        [DllImport("user32.dll")]
        internal static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

    }
}