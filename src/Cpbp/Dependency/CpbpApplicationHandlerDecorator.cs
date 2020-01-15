using Cpbp.Contracts;
using System;
using System.Diagnostics;

namespace Cpbp.Dependency
{
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

                InfoLog(application, $"{application.Name} named application started.");

                if (!string.IsNullOrEmpty(application.Value)) InfoLog(application, $"Value : {application.Value}");

                stopwatch.Start();

                _decoratedHandler.Handle(application);

                stopwatch.Stop();

                application.Performance = stopwatch.Elapsed.TotalSeconds;

                InfoLog(application, $"{application.Name} named application finished.");

                InfoLog(application, $"Application performance : {application.Performance}");

                application.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ErrorLog(application, ex);

                throw ex;
            }
        }

        private void InfoLog(TApplication application, string message)
        {
            Console.WriteLine(message);
            application.Log += Environment.NewLine + message;
        }

        private void ErrorLog(TApplication application, Exception exception)
        {
            stopwatch.Stop();

            application.EndDate = DateTime.UtcNow;
            application.Performance = stopwatch.Elapsed.TotalSeconds;
            application.Log += Environment.NewLine + exception.ToString();
            application.Exception = exception;
            application.IsSuccess = false;
            Console.WriteLine(application.Log);
        }
    }
}
