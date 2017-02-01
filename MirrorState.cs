using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEAMTCMirror
{
    public class MirrorState
    {
        private bool active;

        public enum Modes {Null, Hotkey, Button};
        public enum MirrorTypes { Screenshot, Window };

        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                ActiveStatusChanged?.Invoke(this, active);
            }
        }

        private event EventHandler<bool> ActiveStatusChanged;

        public Process SelectedProcess { get; set; }

        public MirrorTypes MirrorType { get; set; }
    }
}
