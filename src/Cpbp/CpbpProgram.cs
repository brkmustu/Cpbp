using System;

namespace Cpbp
{
    /// <summary>
    /// Cpbp cli application base. The "Program" class in the console application must be inherited from this class.
    /// </summary>
    public class CpbpProgram : IDisposable
    {
        /// <summary>
        /// The name of the method that will run the application partition in the interface (Cpbp.Dependency.ICpbpApplicationHandler<>).
        /// </summary>
        private const string CliApplicationMethodName = "Handle";

        private object _applicationHandlerInstance = null;
        private object _applicationInstance = null;

        public CpbpProgram(
                object applicationHandlerInstance,
                object applicationInstance
            )
        {
            _applicationHandlerInstance = applicationHandlerInstance;
            _applicationInstance = applicationInstance;
            if (applicationHandlerInstance == null || applicationInstance == null) throw new ArgumentNullException("The application cannot be run because a valid application name has not been entered");
        }

        /// <summary>
        /// Execute cpbp application specified in cli arguments.
        /// </summary>
        public void Run()
        {
            if (_applicationHandlerInstance != null)
            {
                var method = _applicationHandlerInstance.GetType().GetMethod(CliApplicationMethodName);

                method.Invoke(_applicationHandlerInstance, new object[] { _applicationInstance });
            }
        }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        public bool IsDisposed;

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}
