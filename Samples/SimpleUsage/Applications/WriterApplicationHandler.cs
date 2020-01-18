using Cpbp.Dependency;
using System;

namespace SimpleUsage.Applications
{
    /// <summary>
    /// this class contains blocks of code that are intended to be executed in the application section
    /// </summary>
    public class WriterApplicationHandler : ICpbpApplicationHandler<WriterApplication>
    {
        public void Handle(WriterApplication application)
        {
            if (string.IsNullOrEmpty(application.ApplicationParameter))
            {
                Console.WriteLine("Application parameter is null.");
            }
            else
            {
                Console.WriteLine(application.ApplicationParameter);
            }
        }
    }
}
