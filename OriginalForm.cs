using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OEAMTCMirror
{
    public partial class OriginalForm : Form
    {

        private MirroredForm _mirroredForm;

        //private static LogWriter _log = new LogWriter();
        private static LogWriter _log = null;
        List<Process> _processes = new List<Process>();
        SecondScreenMirror _secondScreenMirror;
        MirrorIndicator _mirrorIndicator = new MirrorIndicator();
        MenuItem _itemStop = new MenuItem();
        public PinBtn _pinBtnForm;

        List<Process> _openWindows = new List<Process>();

        MirrorState _mirrorState;

        private ContextMenu _notifyContextMenu = new ContextMenu();

        private HotKey HK;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            User32.UnregisterHotKey(this.Handle, 1);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == HotKey.WM_HOTKEY_MSG_ID)
            {
                if ((int)m.WParam == 0)
                {
                    if (!_mirrorState.Active)
                    {
                        StartMirroring();

                    }
                    else
                    {
                        StopMirroring();
                    }
                }
            }

            base.WndProc(ref m);
        }

        public OriginalForm(MirrorState stateObj)
        {
            InitializeComponent();

            _mirrorState = stateObj;

            MenuItem itemClose = new MenuItem();
            MenuItem itemCheckSecond = new MenuItem();

            _notifyContextMenu.MenuItems.Add(itemClose);
            _notifyContextMenu.MenuItems.Add(itemCheckSecond);
            _notifyContextMenu.MenuItems.Add(_itemStop);

            itemClose.Index = 2;
            itemClose.Text = "Quit";
            itemClose.Click += new EventHandler(this.itemClose_Click);

            _itemStop.Index = 0;
            _itemStop.Text = "Stop Mirroring";
            _itemStop.Enabled = false;
            _itemStop.Click += new EventHandler(this.itemStop_Click);

            itemCheckSecond.Index = 1;
            itemCheckSecond.Text = "Show second screen";
            itemCheckSecond.Click += new EventHandler(this.itemCheckSecond_Click);

            notifyIcon1.ContextMenu = _notifyContextMenu;

            _pinBtnForm = new PinBtn(_log, this, _mirrorState);
            _pinBtnForm.Show();


            HK = new HotKey();
            HK.enable(this.Handle, 0, Keys.F8);

            timer1.Stop();

            Screen[] screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                _mirroredForm = new MirroredForm(this);

                _mirroredForm.FormBorderStyle = FormBorderStyle.None;
                _mirroredForm.WindowState = FormWindowState.Maximized;

                _mirroredForm.StartPosition = FormStartPosition.Manual;

                _mirroredForm.Left = screens[0].Bounds.Left + screens[1].Bounds.Left;
            }
            else
            {
                MessageBox.Show("Single monitor not supported\r\nMirror won't work as supposed");

                Environment.Exit(0);
            }

            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }



        private static User32.WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            User32.WINDOWPLACEMENT placement = new User32.WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            User32.GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                GetDesktopWindowsTitlesToPrivateVar();

                User32.Rect rc = new User32.Rect();
                User32.GetWindowRect(_mirrorState.SelectedProcess.MainWindowHandle, ref rc);

                if (!_mirrorState.SelectedProcess.HasExited || _openWindows.Contains(_mirrorState.SelectedProcess))
                {
                    if (User32.IsIconic(_mirrorState.SelectedProcess.MainWindowHandle))
                    {
                        User32.ShowWindow(_mirrorState.SelectedProcess.MainWindowHandle, User32.Restore);
                    }

                    DrawImageToForm();
                }
                else
                {
                    _mirrorState.SelectedProcess = null;
                    timer1.Stop();
                    _mirroredForm.Hide();
                    _mirrorState.Active = false;
                    //_mirrorIndicator.Hide();
                    _itemStop.Enabled = false;
                }
            }
            catch { }
        }


        public Bitmap CaptureApplication(Process proc)
        {
            Bitmap cropped = null;

            try
            {
                //var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height - 25, PixelFormat.Format32bppArgb);
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                //gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.WorkingArea.X, Screen.PrimaryScreen.WorkingArea.Y + 25, 0, 0, Screen.PrimaryScreen.WorkingArea.Size, CopyPixelOperation.SourceCopy);


                //Mouse addition by http://www.codeproject.com/KB/cs/DesktopCaptureWithMouse.aspx?display=Print
                int cursorX = 0;
                int cursorY = 0;
                Bitmap desktopBMP;
                Bitmap cursorBMP;
                Graphics g;
                Rectangle r;

                desktopBMP = bmpScreenshot;

                cursorBMP = CaptureCursor(ref cursorX, ref cursorY);
                if (desktopBMP != null)
                {
                    if (cursorBMP != null)
                    {
                        r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                        g = Graphics.FromImage(desktopBMP);
                        g.DrawImage(cursorBMP, r);
                        g.Flush();

                        var placement = GetPlacement(_mirrorState.SelectedProcess.MainWindowHandle);

                        if (placement.showCmd == User32.ShowWindowCommands.Normal)
                        {
                            //get window rect from process
                            User32.Rect rc = new User32.Rect();
                            User32.GetWindowRect(_mirrorState.SelectedProcess.MainWindowHandle, ref rc);

                            Bitmap original = new Bitmap(desktopBMP);
                            Rectangle srcRect = new Rectangle(rc.left, rc.top, rc.right - rc.left - 50, rc.bottom - rc.top - 50);

                            //Bitmap cropped = (Bitmap)original.Clone(srcRect, original.PixelFormat);
                            cropped = (Bitmap)original.Clone(srcRect, original.PixelFormat);
                        }
                        else
                        {
                            cropped = desktopBMP;
                        }

                        GC.Collect();
                        
                        //return bmpScreenshot;
                        return cropped;

                        //return desktopBMP;
                    }
                    else
                    {
                        var placement = GetPlacement(_mirrorState.SelectedProcess.MainWindowHandle);

                        if (placement.showCmd == User32.ShowWindowCommands.Normal)
                        {

                            //get window rect from process
                            User32.Rect rc = new User32.Rect();
                            User32.GetWindowRect(_mirrorState.SelectedProcess.MainWindowHandle, ref rc);

                            Bitmap original = new Bitmap(desktopBMP);
                            Rectangle srcRect = new Rectangle(rc.left, rc.top, rc.right - rc.left - 50, rc.bottom - rc.top - 50);

                            //Bitmap cropped = (Bitmap)original.Clone(srcRect, original.PixelFormat);
                            cropped = (Bitmap)original.Clone(srcRect, original.PixelFormat);
                        }
                        else
                        {
                            cropped = desktopBMP;
                        }

                        GC.Collect();

                        //return bmpScreenshot;
                        return cropped;

                        //return desktopBMP;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static Bitmap CaptureCursor(ref int x, ref int y)
        {
            try
            {
                //Method by http://www.codeproject.com/KB/cs/DesktopCaptureWithMouse.aspx?display=Print

                Bitmap bmp;
                IntPtr hicon;
                User32.CURSORINFO ci = new User32.CURSORINFO();
                User32.ICONINFO icInfo;
                ci.cbSize = Marshal.SizeOf(ci);
                if (User32.GetCursorInfo(out ci))
                {
                    if (ci.flags == User32.CURSOR_SHOWING)
                    {
                        hicon = User32.CopyIcon(ci.hCursor);
                        if (User32.GetIconInfo(hicon, out icInfo))
                        {
                            x = ci.ptScreenPos.x - ((int)icInfo.xHotspot);
                            y = ci.ptScreenPos.y - ((int)icInfo.yHotspot) - 25;

                            Icon ic = Icon.FromHandle(hicon);
                            bmp = ic.ToBitmap();
                            return bmp;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public Bitmap CaptureSecondScreen()
        {
            try
            {
                //var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height - 25, PixelFormat.Format32bppArgb);
                var bmpScreenshot = new Bitmap(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height, PixelFormat.Format32bppArgb);
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                gfxScreenshot.CopyFromScreen(Screen.AllScreens[1].Bounds.X, Screen.AllScreens[1].Bounds.Y, 0, 0, Screen.AllScreens[1].Bounds.Size, CopyPixelOperation.SourceCopy);

                return bmpScreenshot;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }


        public void DrawImageToForm()
        {
            try
            {
                BringWindowToForeground(_mirrorState.SelectedProcess);
                Bitmap bmp = CaptureApplication(_mirrorState.SelectedProcess);

                this.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
                _mirroredForm.MirrorPictureBox.Image = bmp;
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void DrawSecondScreenToWindow()
        {
            try
            {
                Bitmap bmp = CaptureSecondScreen();

                var newBMP = bmp;

                this.DrawToBitmap(newBMP, new Rectangle(Point.Empty, newBMP.Size));
                _secondScreenMirror.pictureBox1.Image = newBMP;
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(maxWidth, maxHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, maxWidth, maxHeight);

            return newImage;
        }

        private Process GetProcessFromActiveWindow()
        {
            try
            {
                IntPtr foregroundWnd = User32.GetForegroundWindow();
                uint prcId;
                User32.GetWindowThreadProcessId(foregroundWnd, out prcId);
                Process prc = Process.GetProcessById((int)prcId);

                return prc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void BringWindowToForeground(Process process)
        {
            try
            {
                var activeProcess = GetProcessFromActiveWindow();
                var activeProcessID = activeProcess.Id;
                var selectedProcess = _mirrorState.SelectedProcess.Id;

                if (activeProcessID != selectedProcess)
                {
                    // Simulate a key press
                    User32.keybd_event((byte)User32.ALT, 0x45, User32.EXTENDEDKEY | 0, 0);

                    // Simulate a key release
                    User32.keybd_event((byte)User32.ALT, 0x45, User32.EXTENDEDKEY | User32.KEYUP, 0);

                    User32.SetForegroundWindow(_mirrorState.SelectedProcess.MainWindowHandle);
                }

                var placement = GetPlacement(process.MainWindowHandle);

                if (placement.showCmd == User32.ShowWindowCommands.Minimized)
                {
                    //User32.ShowWindowAsync(process.MainWindowHandle, User32.SW_SHOWMAXIMIZED);
                    User32.ShowWindowAsync(process.MainWindowHandle, User32.SW_SHOWNORMAL);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        private bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            try
            {
                string strTitle = GetWindowText(hWnd);
                if (strTitle != "" & User32.IsWindowVisible(hWnd)) //
                {
                    uint prcId;
                    User32.GetWindowThreadProcessId(hWnd, out prcId);
                    _openWindows.Add(Process.GetProcessById((int)prcId));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public string GetWindowText(IntPtr hWnd)
        {
            try
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = User32._GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                strbTitle.Length = nLength;
                return strbTitle.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void GetDesktopWindowsTitlesToPrivateVar()
        {
            try
            {
                User32.EnumDelegate delEnumfunc = new User32.EnumDelegate(EnumWindowsProc);
                bool bSuccessful = User32.EnumDesktopWindows(IntPtr.Zero, delEnumfunc, IntPtr.Zero); //for current desktop
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        public void StartMirroring()
        {
            try
            {
                _mirrorState.SelectedProcess = GetProcessFromActiveWindow();
                DrawImageToForm();

                this.WindowState = FormWindowState.Minimized;

                //Allow 250 milliseconds for the screen to repaint itself (we don't want to include this form in the capture)
                Thread.Sleep(250);

                timer1.Start();
                _mirroredForm.Show();

                _mirroredForm.WindowState = FormWindowState.Maximized;
                _mirroredForm.Activate();

                _mirrorState.Active = true;
                _itemStop.Enabled = true;

                notifyIcon1.Icon = Properties.Resources.icon_active;

                //_mirrorIndicator.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        public void StopMirroring()
        {
            try
            {
                timer1.Stop();
                _mirroredForm.Hide();
                _mirrorState.Active = false;
                _itemStop.Enabled = false;

                _mirrorState.SelectedProcess = null;


                notifyIcon1.Icon = Properties.Resources.notifiyicon;

                //_mirrorIndicator.Hide();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }


        #region Clicks, Hovers, etc.
        private void itemClose_Click(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        private void itemStop_Click(object Sender, EventArgs e)
        {
            StopMirroring();
        }

        private void itemCheckSecond_Click(object Sender, EventArgs e)
        {
            _secondScreenMirror = new SecondScreenMirror(this);
            _secondScreenMirror.timer1.Start();
            _secondScreenMirror.Show();
        }

        private void OriginalForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            _pinBtnForm.Hide();
            if (_mirrorState.Active)
            {
                timer1.Stop();
            }
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            _pinBtnForm.Show();
            if (!_mirrorState.Active)
            {
                timer1.Start();
            }
        }

        #endregion

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (!_mirrorState.Active)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }
    }


}
