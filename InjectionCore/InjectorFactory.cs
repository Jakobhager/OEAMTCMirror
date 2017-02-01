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
        public IFormInjector MakeFormInjector()
        {
            return new FormInjector();
        }
    }
}