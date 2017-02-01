using System;

namespace InjectionCore.WinApi
{
    internal class WinEventHookHandler : IDisposable
    {
        private readonly IntPtr eventHandle;
        // ReSharper disable once NotAccessedField.Local - storing delegate in a var is required
        private readonly ApiWinEventHook.WinEventDelegate eventHandler;

        public WinEventHookHandler(uint processId,
            uint threadId,
            ApiWinEventHook.EventId eventId,
            ApiWinEventHook.WinEventDelegate eventHandler)
        {
            EventId = eventId;
            this.eventHandler = eventHandler;
            eventHandle = AddHook(processId, threadId, eventId, eventHandler);
        }

        // ReSharper disable once ConvertToAutoProperty
        public ApiWinEventHook.EventId EventId { // ReSharper disable once ConvertPropertyToExpressionBody
            get; }

        public void Dispose()
        {
            if (eventHandle == IntPtr.Zero)
                return;

            ApiWinEventHook.UnhookWinEvent(eventHandle);
        }

        public override string ToString()
        {
            return $"{EventId} - {eventHandler}";
        }

        private static IntPtr AddHook(uint processId,
            uint threadId,
            ApiWinEventHook.EventId eventId,
            ApiWinEventHook.WinEventDelegate eventHandler)
        {
            return AddHook(processId, threadId, eventId, eventId, eventHandler);
        }

        private static IntPtr AddHook(uint processId,
            uint threadId,
            ApiWinEventHook.EventId eventIdMin,
            ApiWinEventHook.EventId eventIdMax,
            ApiWinEventHook.WinEventDelegate eventHandler)
        {
            IntPtr handle = ApiWinEventHook.SetWinEventHook(
                eventIdMin, eventIdMax, IntPtr.Zero, eventHandler,
                processId, threadId, ApiWinEventHook.EventSyncContext.WineventOutofcontext);

            return handle;
        }
    }
}