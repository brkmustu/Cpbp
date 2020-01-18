using Cpbp.Contracts;
using System;
using System.Diagnostics;

namespace Cpbp.Dependency
{
    /// <summary>
    /// basic info and error log decorator
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    public class CpbpApplicationHandlerDecorator<TApplication> : ICpbpApplicationHandler<TApplication>
        where TApplication : CpbpApplication
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
                application.StartDate = DateTime.UtcNow;

                InfoLog(application, $"{application.Name} named application part started.");

                if (!string.IsNullOrEmpty(application.ApplicationParameter)) InfoLog(application, $"ApplicationParameter : {application.ApplicationParameter}");

                stopwatch.Start();

                _decoratedHandler.Handle(application);

                stopwatch.Stop();

                application.Performance = stopwatch.Elapsed.TotalSeconds;

                application.EndDate = DateTime.UtcNow;

                InfoLog(application, $"{application.Name} named application part finished.");

                InfoLog(application, $"Performance : {application.Performance}");

                InfoLog(application, $"Start date : {application.StartDate}");
                
                InfoLog(application, $"End date : {application.EndDate}");

                application.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ErrorLog(application, ex);

                if (application.IsThrowException) throw ex;
            }
        }

        private void InfoLog(TApplication application, string message)
        {
            Console.WriteLine(message);
            application.LogBuilder.AppendLine(message);
        }

        private void ErrorLog(TApplication application, Exception exception)
        {
            stopwatch.Stop();

            application.EndDate = DateTime.UtcNow;
            application.Performance = stopwatch.Elapsed.TotalSeconds;
            application.LogBuilder.AppendLine(exception.ToString());
            application.Exception = exception;
            application.IsSuccess = false;
            Console.WriteLine(application.LogBuilder.ToString());
        }
    }
}
