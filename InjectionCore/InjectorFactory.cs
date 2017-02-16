using System.Collections.Generic;
using InjectionCore.Abstractions;
using InjectionCore.Internals;

namespace InjectionCore
{
    /// <summary>
    ///     Creating instances of IInjector
    /// </summary>
    public class InjectorFactory
    {
        /// <summary>
        ///     Creating instance of form injector
        /// </summary>
        /// <returns>FormInjector instance</returns>
        public IFormInjector MakeFormInjector(IEnumerable<IProcessSelector> processSelectors)
        {
            return new FormInjector(processSelectors);
        }
    }
}