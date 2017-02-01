using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using InjectionCore.Abstractions;

namespace InjectionCore.Internals
{
    internal class FormInjector : IFormInjector
    {
        private readonly Dictionary<IntPtr, Thread> injectionDictionary = new Dictionary<IntPtr, Thread>();

        static FormInjector()
        {
            Debug.Print($"{DateTime.Now:G}: This injector create NEW threads for each form!");
        }

        public void Dispose()
        {
            foreach (Thread thread in injectionDictionary.Values)
                thread.Abort();
        }

        public void Inject<TForm>(IEnumerable<IProcessSelector> processSelectors, Func<IntPtr, TForm> formFactory)
            where TForm : InjectableForm
        {
            foreach (IProcessSelector windowSelector in processSelectors)
            {
                foreach (Process process in windowSelector.GetProcesses)
                    Debug.Print(AttachForm(process.MainWindowHandle, formFactory(process.MainWindowHandle))
                        ? $"{DateTime.Now:G}: Attaching form {typeof(TForm).Name} to process {process.ProcessName} success"
                        : $"{DateTime.Now:G}: Attaching form {typeof(TForm).Name} to process {process.ProcessName} failed, see error above");
                windowSelector.ProcessesChange += (sender, e) =>
                {
                    foreach (Process process in windowSelector.GetProcesses)
                        Debug.Print(AttachForm(process.MainWindowHandle, formFactory(process.MainWindowHandle))
                            ? $"{DateTime.Now:G}: Attaching form {typeof(TForm).Name} to process {process.ProcessName} success"
                            : $"{DateTime.Now:G}: Attaching form {typeof(TForm).Name} to process {process.ProcessName} failed, see error above");
                };
            }
        }

        private bool AttachForm(IntPtr ptr, InjectableForm form)
        {
            if (injectionDictionary.ContainsKey(ptr))
            {
                Debug.Print($"{DateTime.Now:G}: Failed to inject form! Reason:\r\n\tForm already injected");
                return false;
            }
            form.Closed += (o, args) => { injectionDictionary.Remove(form.ParentHandle); };
            try
            {
                Thread thread = new Thread(form.ShowAttached);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                injectionDictionary[ptr] = thread;
                Debug.Print(
                    $"{DateTime.Now:G}: Create new thread with injected form! ManagedThreadId: {thread.ManagedThreadId}");
                return true;
                //form.ShowAttached();
                //form.Invoke((Action)form.ShowAttached);
            }
            catch (Exception e)
            {
                Debug.Print($"{DateTime.Now:G}: Failed to inject form! Reason:\r\n\t{e}");
                return false;
            }
        }
    }
}