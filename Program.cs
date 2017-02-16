using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using InjectionCore;
using InjectionCore.Abstractions;

namespace OEAMTCMirror
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InjectorFactory injectorFactory = new InjectorFactory();
            IFormInjector makeFormInjector = injectorFactory.MakeFormInjector(new[]
            {
                new DefaultProcessSelector("explorer", Process.GetCurrentProcess().ProcessName, "devenv",
                    "ApplicationFrameHost", "ScriptedSandbox64")
            });
            MirrorState stateObject = new MirrorState();
            stateObject.Active = false;
            OriginalForm originalForm = new OriginalForm(stateObject, makeFormInjector);
            makeFormInjector.Inject(ptr => new StartMirroringForm(ptr, stateObject, originalForm.StartMirroring, originalForm.StopMirroring));

            Application.Run(originalForm);
        }
    }
}
