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
        public enum Modes {Null, Hotkey, Button};
        private bool _mirrorState;
        private Process _process;

        public bool Active
        {
            get
            {
                return this._mirrorState;
            }
            set
            {
                this._mirrorState = value;
            }
        }

        public Process SelectedProcess
        {
            get
            {
                return this._process;                    
            }
            set
            {
                this._process = value;
            }
        }
    }
}
