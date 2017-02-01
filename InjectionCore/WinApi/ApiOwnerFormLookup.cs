using System;
using System.Runtime.InteropServices;

namespace InjectionCore.WinApi
{
    /// <summary>
    ///     Find owner form of a WinAPI window.
    /// </summary>
    internal class ApiOwnerFormLookup : ParentChildHierarchyLookup<IntPtr>
    {
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        protected override IntPtr GetParentElement(IntPtr child)
        {
            return GetParent(child);
        }

        protected override bool StopCondition(IntPtr parent)
        {
            return parent == IntPtr.Zero;
        }
    }
}