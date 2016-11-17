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
        public enum MirrorTypes { Screenshot, Window };

        private bool _mirrorState;
        private Process _process;
        private MirrorTypes _type;

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

        public MirrorTypes MirrorType
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
    }
}
