using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using InjectionCore.WinApi.WindowHookNet;

namespace InjectionCore.WinApi.Hooks
{
    /// <summary>
    ///     allows you to get information about the creation and /or the destruction of
    ///     windows
    /// </summary>
    public class WindowHookNet
    {
        private const int Maxtitle = 255;
        private static WindowHookNet cInstance;
        private readonly List<WindowHookEventArgs> iEventsToFire = new List<WindowHookEventArgs>();

        private readonly Dictionary<IntPtr, WindowHookEventArgs> iNewWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        private readonly Dictionary<IntPtr, WindowHookEventArgs> iOldWindowList =
            new Dictionary<IntPtr, WindowHookEventArgs>();

        private readonly Thread iThread;
        private bool iRun;

        private WindowHookNet()
        {
            ThreadStart tStart = Run;
            iThread = new Thread(tStart);
        }

        #region properties

        /// <summary>
        ///     made singleton to save up CPU cycles
        /// </summary>
        public static WindowHookNet Instance
        {
            get
            {
                if (null == cInstance)
                    cInstance = new WindowHookNet();

                return cInstance;
            }
        }

        #endregion

        #region Shutdown

        public void Shutdown()
        {
            if (iRun)
                iRun = false;
        }

        #endregion

        #region enumerateWindows

        private void EnumerateWindows()
        {
            EnumDelegate enumfunc = EnumWindowsProc;
            IntPtr hDesktop = IntPtr.Zero; // current desktop
            bool success = _EnumDesktopWindows(hDesktop, enumfunc, IntPtr.Zero);

            if (!success)
            {
                // Get the last Win32 error code
                int errorCode = Marshal.GetLastWin32Error();

                string errorMessage = string.Format(
                    "EnumDesktopWindows failed with code {0}.", errorCode);
                throw new Exception(errorMessage);
            }
        }

        #endregion

        #region EnumWindowsProc

        private bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            WindowHookEventArgs tArgument = new WindowHookEventArgs();

            tArgument.Handle = hWnd;
            tArgument.WindowTitle = GetWindowText(hWnd);
            tArgument.WindowClass = GetClassName(hWnd);

            iNewWindowList.Add(tArgument.Handle, tArgument);
            return true;
        }

        #endregion

        #region fireClosedWindows

        private void FireClosedWindows()
        {
            iEventsToFire.Clear();
            foreach (IntPtr tPtr in iOldWindowList.Keys)
                if (!iNewWindowList.ContainsKey(tPtr))
                    iEventsToFire.Add(iOldWindowList[tPtr]);

            // you need to remove / add things later, because
            // you are not allowed to alter the dictionary during iteration
            foreach (WindowHookEventArgs tArg in iEventsToFire)
            {
                iOldWindowList.Remove(tArg.Handle);
                OnWindowDestroyed(tArg);
            }
        }

        #endregion

        #region fireCreatedWindows

        private void FireCreatedWindows()
        {
            iEventsToFire.Clear();
            foreach (IntPtr tPtr in iNewWindowList.Keys)
                if (!iOldWindowList.ContainsKey(tPtr))
                    iEventsToFire.Add(iNewWindowList[tPtr]);

            // you need to remove / add things later, because
            // you are not allowed to alter the dictionary during iteration
            foreach (WindowHookEventArgs tArg in iEventsToFire)
            {
                iOldWindowList.Add(tArg.Handle, tArg);
                OnWindowCreated(tArg);
            }
        }

        #endregion

        #region run

        private void Run()
        {
            try
            {
                while (iRun)
                {
                    iNewWindowList.Clear();
                    EnumerateWindows();
                    // if the hook has been freshly installed
                    // simply copy the new list to the "old" one
                    if (0 == iOldWindowList.Count)
                    {
                        foreach (KeyValuePair<IntPtr, WindowHookEventArgs> tKvp in iNewWindowList)
                            iOldWindowList.Add(tKvp.Key, tKvp.Value);
                    }
                    else // the hook has been running for some time
                    {
                        FireClosedWindows();
                        FireCreatedWindows();
                    }
                    Thread.Sleep(500);
                }

                // if the hook has been uninstalled
                // delete the list of old windows
                // because when it is restarted you do not want to get a whole 
                // lot of events for windows that where already there

                iOldWindowList.Clear();
            }
            catch (Exception aException)
            {
                Console.Out.WriteLine("exception in thread:" + aException);
            }
        }

        #endregion

        #region delegates

        /// <summary>
        ///     use this to get informed about window creation / destruction events
        /// </summary>
        /// <param name="aSender"></param>
        /// <param name="aArgs">contains information about the window</param>
        public delegate void WindowHookDelegate(object aSender, WindowHookEventArgs aArgs);

        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        #endregion

        #region events

        private event WindowHookDelegate InnerWindowCreated;
        private event WindowHookDelegate InnerWindowDestroyed;

        /// <summary>
        ///     register to this event if you want to be informed about
        ///     the creation of a window
        /// </summary>
        public event WindowHookDelegate WindowCreated
        {
            add
            {
                InnerWindowCreated += value;
                if (!iRun)
                {
                    iRun = true;
                    iThread.Start();
                }
            }
            remove
            {
                InnerWindowCreated -= value;

                // if no more listeners for the events
                if (null == InnerWindowCreated &&
                    null == InnerWindowDestroyed)
                    iRun = false;
            }
        }

        /// <summary>
        ///     register to this event, if you want to be informed about
        ///     the destruction of a window
        /// </summary>
        public event WindowHookDelegate WindowDestroyed
        {
            add
            {
                InnerWindowDestroyed += value;
                if (!iRun)
                {
                    iRun = true;
                    iThread.Start();
                }
            }
            remove
            {
                InnerWindowDestroyed -= value;

                // if no more listeners for the events
                if (null == InnerWindowCreated &&
                    null == InnerWindowDestroyed)
                    iRun = false;
            }
        }

        private void OnWindowCreated(WindowHookEventArgs aArgs)
        {
            if (null != InnerWindowCreated)
                InnerWindowCreated(this, aArgs);
        }

        private void OnWindowDestroyed(WindowHookEventArgs aArgs)
        {
            if (null != InnerWindowDestroyed)
                InnerWindowDestroyed(this, aArgs);
        }

        #endregion

        #region DLLImport

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool _EnumDesktopWindows(IntPtr hDesktop,
            EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int _GetWindowText(IntPtr hWnd,
            StringBuilder lpWindowText, int nMaxCount);


        // GetClassName
        [DllImport("user32.dll", EntryPoint = "GetClassName", ExactSpelling = false,
            CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int _GetClassName(IntPtr hwnd, StringBuilder lpClassName,
            int nMaxCount);

        #endregion

        #region GetWindowText/ClassName

        /// <summary>
        ///     Returns the caption of a window by given HWND identifier.
        /// </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(Maxtitle);
            int titleLength = _GetWindowText(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(Maxtitle);
            int titleLength = _GetClassName(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }

        #endregion
    }
}