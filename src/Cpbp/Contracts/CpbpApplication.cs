using System;
using System.Text;

namespace Cpbp.Contracts
{
    public abstract class CpbpApplication
    {
        public string Value { get; set; }
        public abstract int ExecutationOrder { get; }
        public abstract bool IsRequired { get; }
        public double Performance { get; set; }
        public string Log { get; set; }
        public Exception Exception { get; set; }
        public string Name { get; private set; }
        public bool? IsSuccess { get; set; }
        public bool IsFinished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public CpbpApplication()
        {
            Name = this.GetType().Name;
            IsFinished = false;
        }


        public override string ToString()
        {
            StringBuilder logBuilder = new StringBuilder();

            logBuilder.AppendLine($"Application : {Name}");

            logBuilder.AppendLine($"Is Required : {IsRequired}");

            logBuilder.AppendLine($"Executation Order : {ExecutationOrder}");

            if (!string.IsNullOrEmpty(Value)) logBuilder.AppendLine($"Value (application parameter) : {Value}");

            logBuilder.AppendLine($"Is Finished : {IsFinished}");

            if (IsFinished) logBuilder.AppendLine($"Is Success : {IsSuccess}");

            if (IsSuccess.HasValue && !IsSuccess.Value) logBuilder.AppendLine($"Exception : {Exception}");

            if (!string.IsNullOrEmpty(Log))
            {
                logBuilder.AppendLine($"Other logs :");
                logBuilder.AppendLine(Log);
            }

            Log = logBuilder.ToString();

            return Log;
        }
    }
}
