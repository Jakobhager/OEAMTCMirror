using System;
using System.Collections.Generic;

namespace InjectionCore.Abstractions
{
    /// <summary>
    ///     Injecting form into process in process selector
    /// </summary>
    public interface IFormInjector : IDisposable
    {
        /// <summary>
        ///     Injecting form into process in process selector
        /// </summary>
        /// <param name="formFactory">Delegate to create derived form with parent handle</param>
        /// <typeparam name="TForm">Derived from InjectableForm form</typeparam>
        void Inject<TForm>(Func<IntPtr, TForm> formFactory)
            where TForm : InjectableForm;
    }
}