using Cpbp.Dependency;
using SimpleInjector;
using System;
using System.Reflection;

namespace Cpbp
{
    /// <summary>
    /// Cpbp application base module
    /// You can add the services and classes you want to register here by override the "CustomRegistrations" method.
    /// </summary>
    public class CpbpModule : IDisposable
    {
        /// <summary>
        /// Executing assembly array.
        /// </summary>
        private static Assembly[] assemblies = null;

        /// <summary>
        /// Current Ioc Container (simple injector).
        /// </summary>
        public static Container IocContainer;

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        /// <summary>
        /// Application services registrations helper.
        /// </summary>
        /// <param name="container">Ioc Container (Using simple injector DI framework)</param>
        /// <param name="executingAssembly">cpbp application executing assemblies. assemblies to search for application partition types</param>
        public void Bootstrap(Container container, Assembly[] executingAssembly)
        {
            assemblies = executingAssembly;

            if (container == null) throw new ArgumentNullException(nameof(container));

            IocContainer = container;

            container.Register(typeof(ICpbpApplicationHandler<>), assemblies);

            CustomRegistrations();

            container.RegisterDecorator(typeof(ICpbpApplicationHandler<>), typeof(CpbpApplicationHandlerDecorator<>));

            container.Verify();
        }


        /// <summary>
        /// Custom services registrations region.
        /// </summary>
        public virtual void CustomRegistrations() { }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            assemblies = null;

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}
