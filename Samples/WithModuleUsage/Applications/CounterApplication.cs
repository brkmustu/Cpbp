using Cpbp.Contracts;

namespace WithModuleUsage.Applications
{
    /// <summary>
    /// application part class. this class contains base cpbp application properties
    /// </summary>
    public class CounterApplication : CpbpApplication
    {
        public override int ExecutationOrder => 2;

        public override bool IsRequired => false;
    }
}
