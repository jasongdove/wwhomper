using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace sharperbot
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left, top, right, bottom;
        }

        public static Rectangle GetClientRect(IntPtr hWnd)
        {
            RECT rect;
            GetClientRect(hWnd, out rect);

            var point = new Point();
            ClientToScreen(hWnd, ref point);
            
            return new Rectangle(
                point.X,
                point.Y,
                rect.right - rect.left,
                rect.bottom - rect.top);
        }
    }
}
