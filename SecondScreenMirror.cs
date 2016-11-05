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
    public partial class SecondScreenMirror : Form
    {
        public SecondScreenMirror()
        {
            InitializeComponent();
        }

        private OriginalForm _mainForm;

        public SecondScreenMirror(Form mainfrm)
        {
            InitializeComponent();
            _mainForm = mainfrm as OriginalForm;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _mainForm.DrawSecondScreenToWindow();
        }

        private void SecondScreenMirror_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mainForm._pinBtnForm.Show();
        }

        private void SecondScreenMirror_Load(object sender, EventArgs e)
        { 
            _mainForm._pinBtnForm.Hide();
        }
    }
}
