using System;
using System.Text;

namespace Cpbp.Contracts
{
    /// <summary>
    /// application partition base
    /// </summary>
    public abstract class CpbpApplication
    {
        /// <summary>
        /// if a parameter value is passed for the application, this value will be automatically set to this field by Cpbp
        /// </summary>
        public string ApplicationParameter { get; set; }
        /// <summary>
        /// when multiple application partitions are run with the same command, 
        /// whichever part you want to run first, give that application part the lowest "ExecutationOrder" value
        /// </summary>
        public abstract int ExecutationOrder { get; }
        /// <summary>
        /// here you can specify whether the application section is required or not. 
        /// this will automatically block the execution of the application and throw an error if the required section is not added to the command line.
        /// </summary>
        public abstract bool IsRequired { get; }
        /// <summary>
        /// application partition performance value (in seconds)
        /// </summary>
        public double Performance { get; set; }
        /// <summary>
        /// application partition basic log records
        /// </summary>
        public StringBuilder LogBuilder { get; set; }
        /// <summary>
        /// If an exception is received during application section operation
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// application partition name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// is the operating state of the application section. cpbp sets this value to true if the application partition runs without exception.
        /// </summary>
        public bool? IsSuccess { get; set; }
        /// <summary>
        /// Used to enable or disable exception throw.
        /// </summary>
        public bool IsThrowException { get; set; }
        /// <summary>
        /// application partition start time
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// application partition end time
        /// </summary>
        public DateTime? EndDate { get; set; }

        public CpbpApplication()
        {
            Name = this.GetType().Name;
            IsThrowException = true;
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine($"Application : {Name}");

            strBuilder.AppendLine($"Is Required : {IsRequired}");

            strBuilder.AppendLine($"Executation Order : {ExecutationOrder}");

            if (!string.IsNullOrEmpty(ApplicationParameter)) strBuilder.AppendLine($"Value (application parameter) : {ApplicationParameter}");

            strBuilder.AppendLine($"Is Success : {IsSuccess}");

            if (IsSuccess.HasValue && !IsSuccess.Value) strBuilder.AppendLine($"Exception : {Exception}");

            if (LogBuilder.Length > 0)
            {
                strBuilder.AppendLine($"Other logs :");
                strBuilder.AppendLine(LogBuilder.ToString());
            }

            return strBuilder.ToString();
        }
    }
}
