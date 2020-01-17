using System.Collections.Generic;

namespace Cpbp
{
    /// <summary>
    /// Cpbp application parameters
    /// </summary>
    public class CpbpParameters
    {
        /// <summary>
        /// Cpbp application arguments
        /// </summary>
        public static Dictionary<string, string> Arguments = new Dictionary<string, string>();

        /// <summary>
        /// An array of Cpbp application type names
        /// </summary>
        public static string[] Applications { get; internal set; }
    }
}
