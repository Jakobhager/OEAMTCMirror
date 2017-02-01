using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InjectionCore.WinApi
{
    internal class ApiHotKey
    {
        public static void Clear(Form f, int keyId)
        {
            try
            {
                UnregisterHotKey(f.Handle, keyId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void Set(Form f, Keys key, int keyId)
        {
            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
                modifiers = modifiers | ModAlt;

            if ((key & Keys.Control) == Keys.Control)
                modifiers = modifiers | ModControl;

            if ((key & Keys.Shift) == Keys.Shift)
                modifiers = modifiers | ModShift;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            RegisterHotKey(f.Handle, keyId, modifiers, (int) k);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        #region fields

        public static int ModAlt = 0x1;
        public static int ModControl = 0x2;
        public static int ModShift = 0x4;
        public static int ModWin = 0x8;
        public static int WmHotkey = 0x312;

        #endregion
    }
}