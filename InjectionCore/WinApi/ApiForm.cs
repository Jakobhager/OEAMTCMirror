using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InjectionCore.WinApi
{
    internal class ApiForm
    {
        /// <summary>
        ///     Get form handle at specified coordinates (x,y).
        /// </summary>
        /// <param name="point">Coordinates to search.</param>
        public static IntPtr Select(Point point)
        {
            IntPtr foundWindowHandle = WindowFromPoint(point);

            ApiOwnerFormLookup parentLookup = new ApiOwnerFormLookup();
            IntPtr formHandle = parentLookup.FindParent(foundWindowHandle);

            return formHandle;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point p);
    }
}