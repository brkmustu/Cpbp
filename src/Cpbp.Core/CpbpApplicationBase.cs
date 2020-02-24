using System;
using System.Text;

namespace Cpbp.Core
{
    /// <summary>
    /// application partition base
    /// </summary>
    public abstract class CpbpApplicationBase : ICpbpApplication
    {
        private DateTime _startDate { get; set; }

        /// <summary>
        /// if a parameter value is passed for the application, this value will be automatically set to this field by Cpbp
        /// </summary>
        public string[] ApplicationParameters { get; set; }
        /// <summary>
        /// application partition performance value (in seconds)
        /// </summary>
        public Nullable<double> Performance { get; set; }
        /// <summary>
        /// application partition name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// is the operating state of the application section. cpbp sets this value to true if the application partition runs without exception.
        /// </summary>
        public Nullable<bool> IsSuccess { get; set; }
        /// <summary>
        /// application partition start time
        /// </summary>
        public DateTime StartDate => _startDate;
        /// <summary>
        /// application partition end time
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }

        public CpbpApplicationBase()
        {
            Name = GetType().Name;
            this._startDate = DateTime.Now;
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine($"Application : {Name}");

            strBuilder.AppendLine($"Is Success : {IsSuccess}");

            strBuilder.AppendLine($"Start Date : {StartDate}");

            if (EndDate.HasValue) strBuilder.AppendLine($"End Date : {EndDate.Value}");

            strBuilder.AppendLine($"Performance : {Performance}");

            //if (ApplicationParameters != null && ApplicationParameters.Length > 0)
            //    ApplicationParameters.ToList().ForEach((item) => strBuilder.AppendLine($"Value (application parameter) : {item}"));

            return strBuilder.ToString();
        }
    }
}
