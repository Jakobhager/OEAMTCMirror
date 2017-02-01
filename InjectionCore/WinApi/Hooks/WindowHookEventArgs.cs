using System;

namespace InjectionCore.WinApi
{
    namespace WindowHookNet
    {
        /// <summary>
        ///     stores data about the raised event
        /// </summary>
        public class WindowHookEventArgs
        {
            public IntPtr Handle = IntPtr.Zero;
            public string WindowClass = null;
            public string WindowTitle = null;

            public override string ToString()
            {
                return "[WindowHookEventArgs|Title:" + WindowTitle + "|Class:"
                       + WindowClass + "|Handle:" + Handle + "]";
            }
        }
    }
}