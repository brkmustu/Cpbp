using Cpbp.Contracts;

namespace SimpleUsage.Applications
{
    /// <summary>
    /// application part class. this class contains base cpbp application properties
    /// </summary>
    public class WriterApplication : CpbpApplication
    {
        public override int ExecutationOrder => 1;

        public override bool IsRequired => true;
    }
}
