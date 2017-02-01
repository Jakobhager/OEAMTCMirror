using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using InjectionCore.WinApi;

namespace InjectionCore
{
    /// <summary>
    ///     Base class for all forms that u may inject into window
    /// </summary>
    public class InjectableForm : Form
    {
        protected internal readonly IntPtr ParentHandle;
        private readonly WinEventHook winEventHook;

        /// <summary>
        /// </summary>
        /// <param name="parentHandle">Handle of window to inject</param>
        public InjectableForm(IntPtr parentHandle) : this()
        {
            winEventHook = new WinEventHook(ParentHandle);
            //SetParent(Handle, ParentHandle);
            ParentHandle = parentHandle;
            Load += (sender, args) =>
            {
                winEventHook.Add(ApiWinEventHook.EventId.EventObjectLocationchange, ParentForm_Moved);
                winEventHook.Add(ApiWinEventHook.EventId.EventSystemMovesizeend, ParentForm_Moved);
                winEventHook.Add(ApiWinEventHook.EventId.EventObjectDestroy, ParentForm_Destroyed);
                winEventHook.Add(ApiWinEventHook.EventId.EventObjectHide, ParentForm_Hide);
                winEventHook.Add(ApiWinEventHook.EventId.EventObjectShow, ParentForm_Show);
                MoveToParentWindow();
            };
        }

        /// <summary>
        ///     Design-time constructor
        ///     DO NOT USE this constructor in public app
        /// </summary>
        public InjectableForm()
        {
            InitializeComponents();
        }

        /// <summary>
        ///     Showing current attachable form, attached to parent window
        /// </summary>
        public void ShowAttached()
        {
            try
            {
                NativeWindow nativeWindow = new NativeWindow();
                nativeWindow.AssignHandle(ParentHandle);
                Show(nativeWindow);
                Application.Run();
                nativeWindow.ReleaseHandle();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            winEventHook.Dispose();
        }

        /// <summary>
        ///     Override this method to set custom position of your form in parent window
        /// </summary>
        /// <param name="parentRectangle">Rectangle that represent parent window</param>
        protected virtual void MoveToParent(Rectangle parentRectangle)
        {
            Left = (int) (parentRectangle.Left + (parentRectangle.Right - parentRectangle.Left) * 0.75);
            Top = parentRectangle.Top + 5;
        }

        /// <summary>
        ///     Override in child form for custom
        /// </summary>
        protected virtual void MoveToParentWindow()
        {
            Rectangle parentRectangle = ApiWindowPos.Get(ParentHandle);
            MoveToParent(parentRectangle);
        }

        private void InitializeComponents()
        {
            SuspendLayout();
            FormBorderStyle = FormBorderStyle.None;

            //TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private void ParentForm_Destroyed(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != ParentHandle)
                return;
            Close();
        }

        private void ParentForm_Hide(IntPtr hwineventhook, uint eventtype, IntPtr hwnd, int idobject, int idchild,
            uint dweventthread, uint dwmseventtime)
        {
            if (hwnd != ParentHandle)
                return;
            Visible = false;
        }

        private void ParentForm_Moved(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != ParentHandle)
                return;
            MoveToParentWindow();
        }

        private void ParentForm_Show(IntPtr hwineventhook, uint eventtype, IntPtr hwnd, int idobject, int idchild,
            uint dweventthread, uint dwmseventtime)
        {
            if (hwnd != ParentHandle)
                return;
            Visible = true;
        }
    }
}