using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InjectionCore.Abstractions;
using InjectionCore.WinApi.Hooks;

namespace InjectionCore
{
    /// <summary>
    ///     Default implementation of IProcessSelector
    ///     Return windows for all process except excluded
    /// </summary>
    public class DefaultProcessSelector : IProcessSelector
    {
        private readonly string[] excludedProcesses;
        private readonly WindowHookNet windowHookNet = WindowHookNet.Instance;

        /// <summary>
        /// </summary>
        /// <param name="excludedProcesses">Name of processes to exclude</param>
        public DefaultProcessSelector(params string[] excludedProcesses)
        {
            this.excludedProcesses = excludedProcesses;
            windowHookNet.WindowCreated += (sender, args) =>
            {
                if (args.Handle != IntPtr.Zero)
                    ProcessesChange?.Invoke(this, EventArgs.Empty);
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            windowHookNet.Shutdown();
        }

        /// <summary>
        ///     Return processes with window
        /// </summary>
        public IEnumerable<Process> GetProcesses
            =>
                Process.GetProcesses()
                    .Where(
                        process =>
                            excludedProcesses.All(s => s != process.ProcessName) &&
                            process.MainWindowHandle != IntPtr.Zero);

        /// <summary>
        ///     Invoked when new window is created
        /// </summary>
        public event EventHandler ProcessesChange;
    }
}