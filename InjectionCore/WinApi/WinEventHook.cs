using System;
using System.Collections.Generic;

namespace InjectionCore.WinApi
{
    internal class WinEventHook : IDisposable
    {
        private readonly List<WinEventHookHandler> handlers;
        private readonly uint processId;
        private readonly uint threadId;
        private IntPtr handle;

        public WinEventHook(IntPtr handle)
        {
            this.handle = handle;
            handlers = new List<WinEventHookHandler>();
            threadId = ApiWinEventHook.GetWindowThreadProcessId(handle, out processId);
        }

        public void Dispose()
        {
            foreach (WinEventHookHandler handler in handlers)
                handler.Dispose();
        }

        public void Add(ApiWinEventHook.EventId eventId, ApiWinEventHook.WinEventDelegate eventHandler)
        {
            WinEventHookHandler newHandler = new WinEventHookHandler(processId, threadId, eventId, eventHandler);
            handlers.Add(newHandler);
        }
    }
}