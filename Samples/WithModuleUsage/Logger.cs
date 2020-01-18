using System;

namespace WithModuleUsage
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message + " - LogDate : " + DateTime.Now.ToString());
        }
    }
}
