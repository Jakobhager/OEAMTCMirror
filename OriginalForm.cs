﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using InjectionCore.Abstractions;

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


        private List<string> _defaultScreenshotWindows = new List<string>(new string[] { "chrome", "whatsapp", "iexplore", "swtor", "client", "opera", "vivaldi", "skype", "notepad" });
        public List<string> _defaultExcludedWindows = new List<string>(new string[] { "explorer", Process.GetCurrentProcess().ProcessName });
        private Keys _defaultHotkey = Keys.F8;
        private int _defaultMirrorIndex = Screen.AllScreens.Length;


        private static string _registryFolder = "OEAMTCMirror";


        RegistryKey folderKeyMachine = Registry.LocalMachine.CreateSubKey("SOFTWARE\\" + _registryFolder);
        RegistryKey folderKeyUser = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + _registryFolder);


        List<Process> _openWindows = new List<Process>();
        MirrorState _mirrorState;
        private readonly IFormInjector injector;
        private ContextMenu _notifyContextMenu = new ContextMenu();
        private HotKey HK;
        private bool _clicked = false;



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

        public OriginalForm(MirrorState stateObj, IFormInjector injector)
        {
            InitializeComponent();

            _mirrorState = stateObj;
            this.injector = injector;

            MenuItem itemClose = new MenuItem();
            MenuItem itemCheckSecond = new MenuItem();
            MenuItem itemMoveWindows = new MenuItem();

            _notifyContextMenu.MenuItems.Add(itemClose);
            _notifyContextMenu.MenuItems.Add(itemCheckSecond);
            _notifyContextMenu.MenuItems.Add(itemMoveWindows);
            _notifyContextMenu.MenuItems.Add(_itemStop);

            itemClose.Index = 3;
            itemClose.Text = "Schließen";
            itemClose.Click += new EventHandler(this.itemClose_Click);

            _itemStop.Index = 0;
            _itemStop.Text = "Spiegelung beenden";
            _itemStop.Enabled = false;
            _itemStop.Click += new EventHandler(this.itemStop_Click);

            itemCheckSecond.Index = 1;
            itemCheckSecond.Text = "Zweiten Bildschirm anzeigen";
            itemCheckSecond.Click += new EventHandler(this.itemCheckSecond_Click);

            itemMoveWindows.Index = 2;
            itemMoveWindows.Text = "Alle Fenster auf Hauptbildschirm schieben";
            itemMoveWindows.Click += new EventHandler(this.itemMoveWindows_Click);

            notifyIcon1.ContextMenu = _notifyContextMenu;

            //CreatePinBtnForm();
            //_pinBtnForm.PositionButton();

            FirstStartCheck();

            HK = new HotKey();
            HK.enable(this.Handle, 0, GetHotkeyFromRegistry());

            GetExcludedWindowsFromRegistry();
            GetScreenshotWindowsFromRegistry();

            _defaultMirrorIndex = GetMirrorScreenIndexFromRegistry();


            timer1.Stop();

            Screen[] screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                CreateMirrorForm();
            }
            else
            {
                MessageBox.Show("Single monitor nicht unterstützt.\r\nSpiegelung funtkioniert nicht wie gewünscht.");

                Environment.Exit(0);
            }

            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private void CreateMirrorForm()
        {
            Screen[] screens = Screen.AllScreens;

            _mirroredForm = new MirroredForm(this);
            _mirroredForm.WindowState = FormWindowState.Normal;
            _mirroredForm.StartPosition = FormStartPosition.Manual;
            _mirroredForm.Location = screens[_defaultMirrorIndex - 1].WorkingArea.Location;
            _mirroredForm.Size = screens[_defaultMirrorIndex - 1].Bounds.Size;
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
                //Premature check if our process is closed -> stop the mirror!
                if (_mirrorState.SelectedProcess.HasExited)
                {
                    StopMirroring();
                }

                if (GetProcessFromActiveWindow().Id == _mirrorState.SelectedProcess.Id)
                {
                    _openWindows.Clear();
                    GetDesktopWindowsTitlesToPrivateVar();

                    User32.Rect rc = new User32.Rect();
                    User32.GetWindowRect(_mirrorState.SelectedProcess.MainWindowHandle, ref rc);


                    if (!_mirrorState.SelectedProcess.HasExited || _openWindows.Contains(_mirrorState.SelectedProcess))
                    {
                        var placement = GetPlacement(_mirrorState.SelectedProcess.MainWindowHandle);

                        if (_mirrorState.MirrorType == MirrorState.MirrorTypes.Screenshot && placement.showCmd == User32.ShowWindowCommands.Normal)
                        {
                            User32.ShowWindow(_mirrorState.SelectedProcess.MainWindowHandle, User32.SW_SHOW);
                            User32.ShowWindow(_mirrorState.SelectedProcess.MainWindowHandle, User32.SW_SHOWMAXIMIZED);
                        }
                        DrawImageToForm();
                    }
                    else
                    {
                        StopMirroring();
                    }
                }
            }
            catch { }
        }

        public Bitmap CaptureApplication(Process proc)
        {
            try
            {
                Bitmap bmpScreenshot = null;
                if (_defaultScreenshotWindows.Contains(_mirrorState.SelectedProcess.ProcessName.ToLower()))
                {
                    var placement = GetPlacement(_mirrorState.SelectedProcess.MainWindowHandle);

                    if (placement.showCmd == User32.ShowWindowCommands.Normal)
                    {
                        User32.ShowWindowAsync(_mirrorState.SelectedProcess.MainWindowHandle, User32.SW_SHOWMAXIMIZED);
                    }

                    bmpScreenshot = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height - 25, PixelFormat.Format32bppArgb);
                    var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.WorkingArea.X, Screen.PrimaryScreen.WorkingArea.Y + 25, 0, 0, Screen.PrimaryScreen.WorkingArea.Size, CopyPixelOperation.SourceCopy);
                }
                else
                {
                    try
                    {
                        User32.Rect rct = new User32.Rect();
                        User32.GetWindowRect(_mirrorState.SelectedProcess.MainWindowHandle, ref rct);

                        bmpScreenshot = new Bitmap(rct.right - rct.left, rct.bottom - rct.top);

                        Graphics memoryGraphics = Graphics.FromImage(bmpScreenshot);

                        IntPtr dc = memoryGraphics.GetHdc();
                        bool success = User32.PrintWindow(_mirrorState.SelectedProcess.MainWindowHandle, dc, 1);
                        memoryGraphics.ReleaseHdc(dc);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }
                }

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

                        GC.Collect();

                        return desktopBMP;
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
                int index = Screen.AllScreens.Length - 1;
                var bmpScreenshot = new Bitmap(Screen.AllScreens[index].Bounds.Width, Screen.AllScreens[index].Bounds.Height, PixelFormat.Format32bppArgb);
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                gfxScreenshot.CopyFromScreen(Screen.AllScreens[index].Bounds.X, Screen.AllScreens[index].Bounds.Y, 0, 0, Screen.AllScreens[index].Bounds.Size, CopyPixelOperation.SourceCopy);

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
                Bitmap bmp = CaptureApplication(_mirrorState.SelectedProcess);

                string stamp = DateTime.Now.ToString("hh.mm.ss.ffffff");

                //this.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));

                if (_clicked)
                {
                    Thread.Sleep(200);
                    _clicked = false;
                }

                _mirroredForm.MirrorPictureBox.Height = bmp.Height;
                _mirroredForm.MirrorPictureBox.Width = bmp.Width;

                _mirroredForm.MirrorPictureBox.Image = bmp;

                //_pinBtnForm.PositionButton();


                GC.Collect();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //_log.Log(ex.ToString());
                //_pinBtnForm.PositionButton();
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

                if (activeProcessID != selectedProcess && _mirrorState.MirrorType == MirrorState.MirrorTypes.Screenshot)
                {
                    //// Simulate a key press
                    //User32.keybd_event((byte)User32.ALT, 0x45, User32.EXTENDEDKEY | 0, 0);

                    //// Simulate a key release
                    //User32.keybd_event((byte)User32.ALT, 0x45, User32.EXTENDEDKEY | User32.KEYUP, 0);

                    User32.SetForegroundWindow(_mirrorState.SelectedProcess.MainWindowHandle);
                }

                var placement = GetPlacement(process.MainWindowHandle);

                User32.ShowWindow(process.MainWindowHandle, User32.SW_SHOWMAXIMIZED);
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

        public void MoveWindows()
        {
            foreach (Process prc in _openWindows)
            {
                if (prc.ProcessName == "explorer")
                {
                    User32.Rect rc = new User32.Rect();
                    User32.GetWindowRect(prc.MainWindowHandle, ref rc);
                }
                User32.SetWindowPos(prc.MainWindowHandle, IntPtr.Zero, 0, 0, 0, 0, User32.SetWindowPosFlags.DoNotChangeOwnerZOrder | User32.SetWindowPosFlags.IgnoreResize);

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

        internal void SelectedProcessExited(object sender, System.EventArgs e)
        {
            StopMirroring();
        }

        public void StartMirroring()
        {
            try
            {
                Thread.Sleep(200);
                if (_mirroredForm == null)
                {
                    CreateMirrorForm();
                }

                _clicked = true;

                uint processID;
                IntPtr foregroundWnd = User32.GetForegroundWindow();
                User32.GetWindowThreadProcessId(foregroundWnd, out processID);
                Process prc = Process.GetProcessById((int)processID);

                _mirrorState.SelectedProcess = prc;

                if (_defaultScreenshotWindows.Contains(_mirrorState.SelectedProcess.ProcessName))
                {
                    _mirrorState.MirrorType = MirrorState.MirrorTypes.Screenshot;
                    User32.ShowWindow(_mirrorState.SelectedProcess.MainWindowHandle, User32.SW_SHOWMAXIMIZED);
                }
                else
                {
                    _mirrorState.MirrorType = MirrorState.MirrorTypes.Window;
                }
                if (InvokeRequired)
                {
                    Invoke((Action)(() =>
                    {
                        WindowState = FormWindowState.Minimized;
                        Thread.Sleep(250);

                        DrawImageToForm();

                        timer1.Start();
                        _mirroredForm.Show();

                        _mirrorState.Active = true;
                        _itemStop.Enabled = true;

                        notifyIcon1.Icon = Properties.Resources.icon_active;
                    }));
                }
                else
                {
                    this.WindowState = FormWindowState.Minimized;
                    Thread.Sleep(250);

                    DrawImageToForm();

                    timer1.Start();
                    _mirroredForm.Show();

                    _mirrorState.Active = true;
                    _itemStop.Enabled = true;

                    notifyIcon1.Icon = Properties.Resources.icon_active;
                }

                GC.Collect();
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
                if (_mirrorState.SelectedProcess?.ProcessName.ToLower() != "iexplore")
                {
                    Thread.Sleep(200);
                }

                timer1.Stop();
                _mirrorState.Active = false;
                _itemStop.Enabled = false;

                _mirrorState.SelectedProcess = null;

                if (_mirroredForm == null)
                    return;

                if (_mirroredForm.InvokeRequired)
                {
                    _mirroredForm.Invoke((Action)(() =>
                   {
                       _mirroredForm.Close();
                       _mirroredForm = null;
                       notifyIcon1.Icon = Properties.Resources.notifiyicon;
                   }));
                }
                else
                {
                    _mirroredForm.Close();
                    _mirroredForm = null;
                    notifyIcon1.Icon = Properties.Resources.notifiyicon;
                }




                //_pinBtnForm.Close();
                //CreatePinBtnForm();

                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        private void FirstStartCheck()
        {
            string checkFirstStart = (string)folderKeyMachine.GetValue("Initiated");

            if (checkFirstStart == null)
            {
                folderKeyMachine.SetValue("Initiated", "true");
            }
        }

        private int GetMirrorScreenIndexFromRegistry()
        {
            int index = _defaultMirrorIndex;

            string machVal = (string)folderKeyMachine.GetValue("MirrorIndex");
            if (machVal != null)
            {
                index = Int32.Parse(machVal);
            }

            string usrVal = (string)folderKeyUser.GetValue("MirrorIndex");
            if (usrVal != null)
            {
                index = Int32.Parse(usrVal);
            }

            if (index > Screen.AllScreens.Length)
            {
                return Screen.AllScreens.Length;
            }
            else
            {
                return index;
            }
        }

        private Keys GetHotkeyFromRegistry()
        {
            Keys key = _defaultHotkey;

            string machVal = (string)folderKeyMachine.GetValue("Hotkey");
            if (machVal != null)
            {
                key = (Keys)Enum.Parse(typeof(Keys), machVal);
            }

            string usrVal = (string)folderKeyUser.GetValue("Hotkey");
            if (usrVal != null)
            {
                key = (Keys)Enum.Parse(typeof(Keys), usrVal);
            }

            return key;
        }

        private void GetExcludedWindowsFromRegistry()
        {
            string[] machVal = (string[])folderKeyMachine.GetValue("ExcludedWindows");
            if (machVal != null)
            {
                _defaultExcludedWindows = new List<string>(machVal);
            }

            string[] usrVal = (string[])folderKeyUser.GetValue("ExcludedWindows");
            if (usrVal != null)
            {
                _defaultExcludedWindows = new List<string>(usrVal);
            }

            _defaultExcludedWindows.Add(Process.GetCurrentProcess().ProcessName);
        }

        private void GetScreenshotWindowsFromRegistry()
        {
            string[] machVal = (string[])folderKeyMachine.GetValue("ScreenshotWindows");
            if (machVal != null)
            {
                _defaultScreenshotWindows = new List<string>(machVal);
            }

            string[] usrVal = (string[])folderKeyUser.GetValue("ScreenshotWindows");
            if (usrVal != null)
            {
                _defaultScreenshotWindows = new List<string>(usrVal);
            }
        }

        #region Clicks, Hovers, etc.
        private void itemClose_Click(object Sender, EventArgs e)
        {
            injector.Dispose();
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

        private void itemMoveWindows_Click(object Sender, EventArgs e)
        {
            GetDesktopWindowsTitlesToPrivateVar();
            MoveWindows();
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
            //_pinBtnForm.Hide();
            if (_mirrorState.Active)
            {
                timer1.Stop();
            }
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            //_pinBtnForm.Show();
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

        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            injector.Dispose();
            Application.Exit();
        }

        private void OriginalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            User32.UnregisterHotKey(this.Handle, 1);
        }
    }
}
