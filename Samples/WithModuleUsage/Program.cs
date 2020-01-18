using Cpbp;

namespace WithModuleUsage
{
    class Program : CpbpProgram
    {
        static void Main(string[] args)
        {
            /// Use only one method at the time of application execution. 
            /// otherwise cpbp will throw an error. 
            /// The infrastructure is built upon calling the "ProgramStart" method once.

            //
            //WithModuleUsage_NoParameter(args);

            //
            //WithModuleUsage_WithParameter(args);
        }

        /// <summary>
        /// method to use your own module in your cli application. Does not contain any additional parameters
        /// </summary>
        static void WithModuleUsage_NoParameter(string[] args)
        {
            var module = new SampleModule(); /// custom module passing parameter "ProgramStart" metod.
            args = new string[] { "--" + typeof(Applications.CounterApplication).Name }; /// to manipulate our own sample cpbp application partition
            ProgramStart(args, new System.Reflection.Assembly[] { typeof(Program).Assembly }, module);
        }

        /// <summary>
        /// method to use your own module in your cli application. Includes the use of parameters
        /// </summary>
        static void WithModuleUsage_WithParameter(string[] args)
        {
            var module = new SampleModule(); /// custom module passing parameter "ProgramStart" metod.
            args = new string[] { "--" + typeof(Applications.CounterApplication).Name, "application parameter example" }; /// to manipulate our own sample cpbp application partition
            ProgramStart(args, new System.Reflection.Assembly[] { typeof(Program).Assembly }, module);
        }
    }
}
