﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InjectionCore.WinApi
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///     Taken from:
    ///     http://stackoverflow.com/questions/16484894/form-tells-wrong-size-on-windows-8-how-to-get-real-size
    /// </remarks>
    internal static class ApiWindowPos
    {
        /// <summary>
        ///     Get real window size, no matter whether Win XP, Win Vista, 7 or 8.
        /// </summary>
        public static Rectangle Get(IntPtr handle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return GetWindowRect(handle);
            }
            Rectangle rectangle;
            return DWMWA_EXTENDED_FRAME_BOUNDS(handle, out rectangle) ? rectangle : GetWindowRect(handle);
        }

        [DllImport(@"dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute,
            int cbAttribute);

        private static bool DWMWA_EXTENDED_FRAME_BOUNDS(IntPtr handle, out Rectangle rectangle)
        {
            Rect rect;
            int result = DwmGetWindowAttribute(handle, (int) Dwmwindowattribute.DwmwaExtendedFrameBounds,
                out rect, Marshal.SizeOf(typeof(Rect)));
            rectangle = rect.ToRectangle();
            return result >= 0;
        }

        [DllImport(@"user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        private static Rectangle GetWindowRect(IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);
            return rect.ToRectangle();
        }

        private enum Dwmwindowattribute
        {
            DwmwaExtendedFrameBounds = 9
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            // ReSharper restore FieldCanBeMadeReadOnly.Local
            // ReSharper restore MemberCanBePrivate.Local

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }
    }
}