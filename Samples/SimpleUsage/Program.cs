using Cpbp;

namespace SimpleUsage
{
    class Program : CpbpProgram
    {
        static void Main(string[] args)
        {
            /// Use only one method at the time of application execution. 
            /// otherwise cpbp will throw an error. 
            /// The infrastructure is built upon calling the "ProgramStart" method once.

            //
            //SimpleUsage_NoParameter(args);

            //
            //SimpleUsage_WithParameter(args);
        }

        /// <summary>
        /// The method you can use to directly launch your cli application without any additional settings.
        /// Does not contain any additional parameters.
        /// </summary>
        static void SimpleUsage_NoParameter(string[] args)
        {
            args = new string[] { "--" + typeof(Applications.WriterApplication).Name }; /// to manipulate our own sample cpbp application partition
            ProgramStart(args, new System.Reflection.Assembly[] { typeof(Program).Assembly });
        }

        /// <summary>
        /// The method you can use to directly launch your cli application without any additional settings.
        /// Does not contain any additional parameters.
        /// </summary>
        static void SimpleUsage_WithParameter(string[] args)
        {
            args = new string[] { "--" + typeof(Applications.WriterApplication).Name, "this is an parameter example" }; /// to manipulate our own sample cpbp application partition
            ProgramStart(args, new System.Reflection.Assembly[] { typeof(Program).Assembly });
        }
    }
}
