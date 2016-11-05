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
    public partial class MirrorIndicator : Form
    {
        public MirrorIndicator()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - 160, workingArea.Bottom - 50);
            InitializeComponent();

            this.ShowInTaskbar = false;
        }
    }
}
