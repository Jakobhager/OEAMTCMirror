using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OEAMTCMirror
{
    class HotKey
    {
        // Taken from http://www.mycsharp.de/wbb2/thread.php?threadid=108903&threadview=1&hilight=&hilightuser=0&sid=1114a0c8f0443f202572d2d211c1d81a
        // Needs some reworking


        //Registers a hotkey to the form hWnd, id unique, modifiers look below, vk is key
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        //Unregisters the hotkey
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //Messagetype that needss to be captured for keydown in WndProc https://msdn.microsoft.com/en-us/library/windows/desktop/ms646279(v=vs.85).aspx
        public const int WM_HOTKEY_MSG_ID = 0x0312;

        //All active hooked keys
        private List<HookInfo> hookedKeys;

        //The next free id
        private int freeID;

        //Contains info about the hook
        public struct HookInfo
        {
            public int ID;
            public IntPtr hWnd;

            public HookInfo(IntPtr Handle, int id)
            {
                ID = id;
                hWnd = Handle;
            }
        }

        //Possible modifiers
        [Flags]
        public enum Modifiers : int
        {
            Win = 8,
            Shift = 4,
            Ctrl = 2,
            Alt = 1,
            None = 0
        }

        //Returns array with all hooked keys
        public HookInfo[] HookedKeys
        {
            get
            {
                return hookedKeys.ToArray();
            }
        }

        public HotKey()
        {
            hookedKeys = new List<HookInfo>();
            freeID = 0;
        }

        ~HotKey()
        {
            unhookAll();
        }

        //Disables all registerd hotkeys
        public void unhookAll()
        {
            for (int i = 0; i < hookedKeys.Count; i++)
            {
                disable(hookedKeys[i]);
            }
        }

        //Creates new hotkey
        public HookInfo enable(IntPtr Handle, Modifiers Mod, Keys Key)
        {
            HookInfo i = new HookInfo(Handle, freeID++);
            hookedKeys.Add(i);
            RegisterHotKey(Handle, i.ID, (int)Mod, (int)Key);
            return i;
        }

        //Removes a hotkey
        public void disable(HookInfo i)
        {
            RemoveHook(i);
            UnregisterHotKey(i.hWnd, i.ID);
        }

        //Removes a specific hotkey from the list of registered keys
        private void RemoveHook(HookInfo hInfo)
        {
            for (int i = 0; i < hookedKeys.Count; i++)
            {
                if (hookedKeys[i].hWnd == hInfo.hWnd && hookedKeys[i].ID == hInfo.ID)
                {
                    hookedKeys.RemoveAt(i--);
                }
            }
        }
    }
}
