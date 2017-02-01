using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InjectionCore.WinApi
{
    public static class ApiWinEventHook
    {
        public delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        public enum EventId : uint
        {
            EventMin = 0x00000001,
            EventMax = 0x7FFFFFFF,
            EventSystemSound = 0x0001,
            EventSystemAlert = 0x0002,
            EventSystemForeground = 0x0003,
            EventSystemMenustart = 0x0004,
            EventSystemMenuend = 0x0005,
            EventSystemMenupopupstart = 0x0006,
            EventSystemMenupopupend = 0x0007,
            EventSystemCapturestart = 0x0008,
            EventSystemCaptureend = 0x0009,
            EventSystemMovesizestart = 0x000A,
            EventSystemMovesizeend = 0x000B,
            EventSystemContexthelpstart = 0x000C,
            EventSystemContexthelpend = 0x000D,
            EventSystemDragdropstart = 0x000E,
            EventSystemDragdropend = 0x000F,
            EventSystemDialogstart = 0x0010,
            EventSystemDialogend = 0x0011,
            EventSystemScrollingstart = 0x0012,
            EventSystemScrollingend = 0x0013,
            EventSystemSwitchstart = 0x0014,
            EventSystemSwitchend = 0x0015,
            EventSystemMinimizestart = 0x0016,
            EventSystemMinimizeend = 0x0017,
            EventSystemDesktopswitch = 0x0020,
            EventSystemEnd = 0x00FF,
            EventOemDefinedStart = 0x0101,
            EventOemDefinedEnd = 0x01FF,
            EventUiaEventidStart = 0x4E00,
            EventUiaEventidEnd = 0x4EFF,
            EventUiaPropidStart = 0x7500,
            EventUiaPropidEnd = 0x75FF,
            EventConsoleCaret = 0x4001,
            EventConsoleUpdateRegion = 0x4002,
            EventConsoleUpdateSimple = 0x4003,
            EventConsoleUpdateScroll = 0x4004,
            EventConsoleLayout = 0x4005,
            EventConsoleStartApplication = 0x4006,
            EventConsoleEndApplication = 0x4007,
            EventConsoleEnd = 0x40FF,
            EventObjectCreate = 0x8000, // hwnd ID idChild is created item
            EventObjectDestroy = 0x8001, // hwnd ID idChild is destroyed item
            EventObjectShow = 0x8002, // hwnd ID idChild is shown item
            EventObjectHide = 0x8003, // hwnd ID idChild is hidden item
            EventObjectReorder = 0x8004, // hwnd ID idChild is parent of zordering children
            EventObjectFocus = 0x8005, // hwnd ID idChild is focused item
            EventObjectSelection = 0x8006,
            // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
            EventObjectSelectionadd = 0x8007, // hwnd ID idChild is item added
            EventObjectSelectionremove = 0x8008, // hwnd ID idChild is item removed
            EventObjectSelectionwithin = 0x8009, // hwnd ID idChild is parent of changed selected items
            EventObjectStatechange = 0x800A, // hwnd ID idChild is item w/ state change
            EventObjectLocationchange = 0x800B, // hwnd ID idChild is moved/sized item
            EventObjectNamechange = 0x800C, // hwnd ID idChild is item w/ name change
            EventObjectDescriptionchange = 0x800D, // hwnd ID idChild is item w/ desc change
            EventObjectValuechange = 0x800E, // hwnd ID idChild is item w/ value change
            EventObjectParentchange = 0x800F, // hwnd ID idChild is item w/ new parent
            EventObjectHelpchange = 0x8010, // hwnd ID idChild is item w/ help change
            EventObjectDefactionchange = 0x8011, // hwnd ID idChild is item w/ def action change
            EventObjectAcceleratorchange = 0x8012, // hwnd ID idChild is item w/ keybd accel change
            EventObjectInvoked = 0x8013, // hwnd ID idChild is item invoked
            EventObjectTextselectionchanged = 0x8014, // hwnd ID idChild is item w? test selection change
            EventObjectContentscrolled = 0x8015,
            EventSystemArrangmentpreview = 0x8016,
            EventObjectEnd = 0x80FF,
            EventAiaStart = 0xA000,
            EventAiaEnd = 0xAFFF
        }

        [Flags]
        public enum EventSyncContext
        {
            WineventIncontext = 4,
            WineventOutofcontext = 0,
            WineventSkipownprocess = 2,
            WineventSkipownthread = 1
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(EventId eventMin, EventId eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, EventSyncContext dwFlags);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr eventHookHandle);

        internal delegate void WinEventProc(
            IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId,
            uint eventThreadId, uint eventTimeInMilliseconds);
    }
}