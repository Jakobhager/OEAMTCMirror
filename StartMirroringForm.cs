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

        public StartMirroringForm()
        {
            InitializeComponent();
        }

        public StartMirroringForm(IntPtr parentHandle, MirrorState mirrorState) : base(parentHandle)
        {
            InitializeComponent();
            this.mirrorState = mirrorState;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (mirrorState.Active)
            {
                try
                {
                    mirrorState.SelectedProcess = null;
                    mirrorState.Active = false;
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
                    mirrorState.SelectedProcess =
                        Process.GetProcesses().First(process => process.MainWindowHandle == ParentHandle);
                    mirrorState.Active = true;
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
