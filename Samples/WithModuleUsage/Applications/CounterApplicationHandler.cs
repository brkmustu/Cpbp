using Cpbp.Dependency;
using System;
using System.Threading;

namespace WithModuleUsage.Applications
{
    /// <summary>
    /// this class contains blocks of code that are intended to be executed in the application section
    /// </summary>
    public class CounterApplicationHandler : ICpbpApplicationHandler<CounterApplication>
    {
        ILogger _logger;
        public CounterApplicationHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(CounterApplication application)
        {
            if (!string.IsNullOrEmpty(application.ApplicationParameter)) Console.WriteLine($"application parameter value is : {application.ApplicationParameter}");

            int counter;
            if (!int.TryParse(application.ApplicationParameter, out counter)) counter = 5;

            for (int i = 0; i < counter; i++)
            {
                _logger.Log($"executing number : {i}");
                Thread.Sleep(1000);
            }
        }
    }
}
