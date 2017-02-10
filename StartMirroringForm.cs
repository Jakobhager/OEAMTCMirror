using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InjectionCore;
using OEAMTCMirror.Properties;

namespace OEAMTCMirror
{
    public partial class StartMirroringForm : InjectableForm
    {
        private readonly MirrorState mirrorState;
        private readonly Action startMirroringAction;
        private readonly Action stopMirroringAction;

        public StartMirroringForm()
        {
            InitializeComponent();
        }

        public StartMirroringForm(IntPtr parentHandle, MirrorState mirrorState, Action startMirroringAction, Action stopMirroringAction) : base(parentHandle)
        {
            InitializeComponent();
            this.mirrorState = mirrorState;
            this.startMirroringAction = startMirroringAction;
            this.stopMirroringAction = stopMirroringAction;
            this.ShowInTaskbar = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (mirrorState.Active)
            {
                try
                {
                    if (mirrorState.SelectedProcess.MainWindowHandle != ParentHandle)
                        return;
                    stopMirroringAction();
                    pictureBox1.Image = Resources.btn_bg;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
            else
            {
                try
                {
                    User32.SetForegroundWindow(ParentHandle);
                    startMirroringAction();
                    pictureBox1.Image = Resources.btn_bg_active;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
            
        }
    }
}
