using System;
using System.Diagnostics;

namespace Cpbp.Core
{
    /// <summary>
    /// basic info and error log decorator
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    public class CpbpApplicationHandlerDecorator<TApplication> : ICpbpApplicationHandler<TApplication>
        where TApplication : CpbpApplicationBase
    {
        private readonly ICpbpApplicationHandler<TApplication> _decoratedHandler;

        private Stopwatch stopwatch;

        public CpbpApplicationHandlerDecorator(ICpbpApplicationHandler<TApplication> decoratedHandler)
        {
            _decoratedHandler = decoratedHandler;
        }

        public void Handle(TApplication application)
        {
            stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine($"{application.Name} named application part started.");

                _decoratedHandler.Handle(application);

                stopwatch.Stop();

                application.Performance = stopwatch.Elapsed.TotalSeconds;

                application.EndDate = DateTime.UtcNow;

                application.IsSuccess = true;

                Console.WriteLine($"{application.Name} named application part finished.");

                Console.WriteLine(application.ToString());
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                application.EndDate = DateTime.UtcNow;

                application.Performance = stopwatch.Elapsed.TotalSeconds;

                application.IsSuccess = false;

                Console.WriteLine(application.ToString());

                Console.WriteLine(exception.ToString());

                throw;
            }
        }
    }
}
