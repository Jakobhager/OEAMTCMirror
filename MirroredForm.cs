using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OEAMTCMirror
{
    public partial class MirroredForm : Form
    {
        public MirroredForm()
        {
            InitializeComponent();
        }

        private OriginalForm _mainForm;
        public MirroredForm(Form mainfrm)
        {
            InitializeComponent();
            _mainForm = mainfrm as OriginalForm;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void btnCloseMirror_Click(object sender, EventArgs e)
        {
            this.Hide();
            _mainForm.StopMirroring();
        }
    }
}
