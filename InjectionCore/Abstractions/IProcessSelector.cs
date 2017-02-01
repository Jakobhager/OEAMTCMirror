using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InjectionCore.Abstractions
{
    /// <summary>
    ///     Select process for form injecting
    /// </summary>
    public interface IProcessSelector : IDisposable
    {
        /// <summary>
        ///     Return list of selected processes
        /// </summary>
        IEnumerable<Process> GetProcesses { get; }

        /// <summary>
        ///     Signal, that selected process may change
        /// </summary>
        event EventHandler ProcessesChange;
    }
}