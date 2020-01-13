using SimpleInjector;
using System;
using System.Reflection;

namespace Cpbp.Dependency
{
    /// <summary>
    /// Application services registrations helpers.
    /// </summary>
    public class CpbpBootstrapper : IDisposable
    {
        /// <summary>
        /// Executing assembly array.
        /// </summary>
        private static Assembly[] assemblies = new[] { Assembly.GetExecutingAssembly() };

        /// <summary>
        /// Current container.
        /// </summary>
        public static Container IocContainer;

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        /// <summary>
        /// Application services registrations helper.
        /// </summary>
        /// <param name="container"></param>
        public void Bootstrap(Container container, Assembly[] executingAssembly)
        {
            assemblies = executingAssembly;

            if (container == null) throw new ArgumentNullException(nameof(container));

            IocContainer = container;

            container.Register(typeof(ICpbpApplicationHandler<>), assemblies);

            CustomApplicationDecoratorRegistrations();

            container.RegisterDecorator(typeof(ICpbpApplicationHandler<>), typeof(CpbpApplicationHandlerDecorator<>));

            CustomRegistrations();

            container.Verify();
        }

        /// <summary>
        /// for additional application handler decorator registrations
        /// </summary>
        public virtual void CustomApplicationDecoratorRegistrations() { }

        /// <summary>
        /// Custom services registrations region.
        /// </summary>
        /// <param name="container"></param>
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
