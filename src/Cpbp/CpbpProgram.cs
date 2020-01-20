using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cpbp
{
    /// <summary>
    /// Cpbp cli application base. The "Program" class in the console application must be inherited from this class.
    /// </summary>
    public class CpbpProgram : IDisposable
    {
        /// <summary>
        /// The name of the method that will run the application partition in the interface (Cpbp.Dependency.ICpbpApplicationHandler<>).
        /// </summary>
        private const string CliApplicationMethodName = "Handle";

        /// <summary>
        /// is the suffix name of the types of Cpbp applications to be searched for.
        /// </summary>
        private const string CpbpApplicationRootName = "Application";

        /// <summary>
        ///  is the suffix name of the types of Cpbp application handlers to be searched for.
        /// </summary>
        private const string CpbpApplicationHandlerRootName = "ApplicationHandler";

        /// <summary>
        /// The logical name of the property to store the parameter value that may be used during execution of the Cpbp application partition.
        /// </summary>
        private const string CpbpApplicationParameterPropertyName = "ApplicationParameter";

        /// <summary>
        /// The logical name of the field that will allow Cpbp applications to be sorted.
        /// </summary>
        private const string CpbpApplicationOrderFieldLogicalName = "ExecutationOrder";

        /// <summary>
        /// The logical name of the field that will allow control of required application partitions in Cpbp applications.
        /// </summary>
        private const string CpbpApplicationRequiredFieldLogicalName = "IsRequired";

        /// <summary>
        /// separator for cli application arguments.
        /// </summary>
        private readonly string CpbpArgumentSeperator = "--";

        /// <summary>
        /// parametric types are given in assemblies.
        /// </summary>
        private static List<Type> AssemblyTypes = new List<Type>();

        protected static IEnumerable<Type> GetApplicationTypes() =>
            (
                from type in AssemblyTypes
                where type.Name.EndsWith(CpbpApplicationRootName)
                select type
            );

        protected static IEnumerable<Type> GetApplicationHandlerTypes() =>
            from type in AssemblyTypes
            where type.Name.EndsWith(CpbpApplicationHandlerRootName)
            select type;

        /// <summary>
        /// Runs the application
        /// </summary>
        /// <param name="args">Cli arguments</param>
        /// <param name="assemblies">Ioc container registration assemlies.</param>
        /// <param name="applicationModule">For custom module or ioc registrations (Optional).</param>
        public static void ProgramStart<TProgram>(string[] args, Assembly[] assemblies, CpbpModule applicationModule = null)
            where TProgram : CpbpProgram
        {
            using (CpbpProgram program = Activator.CreateInstance<TProgram>())
            {
                program.IsDisposed = false;

                if (CpbpModule.IocContainer == null) CpbpModule.IocContainer = new Container();

                if (applicationModule == null) applicationModule = new CpbpModule();

                applicationModule.Bootstrap(CpbpModule.IocContainer, assemblies);

                /// assembly types on memory
                assemblies.ToList()
                    .ForEach(
                        x => AssemblyTypes.AddRange(x.GetExportedTypes())
                    );

                program.SetArguments(args);

                program.Run();
            }
        }

        /// <summary>
        /// Separate parameters and application partitions.
        /// </summary>
        /// <param name="args"></param>
        private void SetArguments(string[] args)
        {
            var applications = GetApplicationTypes();

            List<string> arguments = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                bool isContain = false;

                foreach (var application in applications)
                    if (arg.Contains(application.Name))
                        isContain = true;

                if (isContain && arg.Contains(CpbpArgumentSeperator))
                {
                    arguments.Add(arg);

                    if (args.Length > i + 1 && !args[i + 1].Contains(CpbpArgumentSeperator)) CpbpParameters.Arguments.Add(arg, args[i + 1]);
                }
            }

            CpbpParameters.Applications = arguments.ToArray();
        }

        /// <summary>
        /// Executes applications specified in cli arguments.
        /// </summary>
        public virtual void Run()
        {
            var applicationTypes = GetApplicationTypes();

            List<Type> requiredApplications = new List<Type>();

            foreach (var item in applicationTypes)
            {
                var instance = CpbpModule.IocContainer.GetInstance(item);

                var IsRequired = (bool)instance
                            .GetType()
                            .GetProperty(CpbpApplicationRequiredFieldLogicalName)
                            .GetValue(instance);

                if (IsRequired) requiredApplications.Add(item);
            }

            /// check required applications existing
            foreach (var command in requiredApplications)
            {
                if (!CpbpParameters.Applications.Contains(CpbpArgumentSeperator + command.Name))
                {
                    Console.WriteLine();

                    throw new Exception($"You did not call a required command! Command name : {command.Name}");
                }
            }

            var orderedApplications = (
                    from a in applicationTypes
                    orderby ((int)a.GetProperty(CpbpApplicationOrderFieldLogicalName).GetValue(Activator.CreateInstance(a)))
                    select a
                ).ToList();

            /// execute applications
            foreach (var application in CpbpParameters.Applications)
            {
                foreach (var command in orderedApplications)
                {
                    if (application.Equals(CpbpArgumentSeperator + command.Name))
                    {
                        Execute(application, command, CpbpModule.IocContainer.GetInstance(command));
                    }
                }
            }
        }

        /// <summary>
        /// is the method that runs the application partition.
        /// </summary>
        /// <param name="application">cpbp application name (Cpbp is the name of the application partition that inherits the Application class and ends with the suffix "Application")</param>
        /// <param name="applicationType">cpbp application type</param>
        /// <param name="applicationInstance">cpbp application instance</param>
        protected void Execute(string application, Type applicationType, object applicationInstance)
        {
            if (application.Contains(CpbpArgumentSeperator))
            {
                string currentArgument = application.Replace(CpbpArgumentSeperator, "");

                string currentArgumentParameter;
                
                CpbpParameters.Arguments.TryGetValue(application, out currentArgumentParameter);

                var handler = GetApplicationInstance(applicationType);

                if (handler != null)
                {
                    var method = handler.GetType().GetMethod(CliApplicationMethodName);

                    if(!string.IsNullOrEmpty(currentArgumentParameter)) 
                        applicationInstance
                            .GetType()
                            .GetProperty(CpbpApplicationParameterPropertyName)
                            .SetValue(applicationInstance, currentArgumentParameter);
                    
                    method.Invoke(handler, new object[] { applicationInstance });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationType">Type inherited from "CpbpApplication". Not handler type</param>
        /// <returns></returns>
        private object GetApplicationInstance(Type applicationType)
        {
            var handlerTypes = GetApplicationHandlerTypes();

            var instanceProducer = CpbpModule.IocContainer.GetCurrentRegistrations();

            List<Type> implementedInterfaces = new List<Type>();

            List<Type> genericTypeArguments = new List<Type>();

            handlerTypes.ToList().ForEach(x => implementedInterfaces.AddRange(((TypeInfo)x).ImplementedInterfaces));

            implementedInterfaces.ForEach(x => genericTypeArguments.AddRange(x.GenericTypeArguments));

            foreach (var genericTypeArgument in genericTypeArguments)
            {
                if (genericTypeArgument.Equals(applicationType))
                {
                    foreach (var serviceItem in instanceProducer)
                    {
                        if (serviceItem.ServiceType.GenericTypeArguments.Contains(genericTypeArgument))
                        {
                            return CpbpModule.IocContainer.GetInstance(serviceItem.ServiceType);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// get type by name
        /// </summary>
        /// <param name="name">type name</param>
        /// <returns></returns>
        protected Type GetTypeByName(string name)
        {
            return (from types in AssemblyTypes
                    where types.Name == name
                    select types).First();
        }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        public void Dispose()
        {

            if (IsDisposed)
            {
                return;
            }

            AssemblyTypes = null;

            CpbpModule.IocContainer= null;

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}
