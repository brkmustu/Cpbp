using Cpbp.Contracts;
using Cpbp.Dependency;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cpbp
{
    public class CpbpProgram : IDisposable
    {
        private const string CliAppMethodName = "Handle";

        //private static Container IocContainer;

        protected readonly string argumentSeperator = "--";

        protected List<string> clibpApplicationNames = new List<string>();

        protected static List<Type> assemblyTypes = new List<Type>();

        protected static Dictionary<Type, object> GetApplicationTypesAndInstances() =>
            (
                from type in assemblyTypes
                where type.Name.EndsWith("Application")
                select new DictonaryDto
                {
                    Key = type,
                    Value = Activator.CreateInstance(type)
                }
            )
            .OrderBy(x=>x.Value.GetType().GetProperty("ExecutationOrder").GetValue(x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        protected static IEnumerable<Type> GetApplicationHandlerTypes() =>
            from type in assemblyTypes
            where type.Name.EndsWith("ApplicationHandler")
            select type;

        /// <summary>
        /// Runs the application
        /// </summary>
        /// <param name="args">Cli arguments</param>
        /// <param name="assemblies">Ioc container registration assemlies.</param>
        /// <param name="bootstrapper">For custom bootstrapper or ioc registrations (Optional).</param>
        public void ProgramStart(string[] args, Assembly[] assemblies, CpbpBootstrapper bootstrapper = null)
        {
            using (CpbpProgram program = new CpbpProgram())
            {
                IsDisposed = false;

                CpbpParams.Args = args;

                CpbpBootstrapper.IocContainer = new Container();

                if (bootstrapper == null) bootstrapper = new CpbpBootstrapper();

                bootstrapper.Bootstrap(CpbpBootstrapper.IocContainer, assemblies);

                /// assembly types on memory
                assemblies.ToList()
                    .ForEach(
                        x => assemblyTypes.AddRange(x.GetExportedTypes())
                    );

                Run();
            }
        }

        /// <summary>
        /// Executes commands specified in cli arguments.
        /// </summary>
        public virtual void Run()
        {
            var orderedCommands = GetApplicationTypesAndInstances();

            var requiredCommands = orderedCommands
                .Where(
                    x => 
                        ((bool)x.Value
                            .GetType()
                            .GetProperty("IsRequired")
                            .GetValue(x.Value))
                            .Equals(true)
                ).ToList();

            /// executes commands sequentially
            foreach (var command in requiredCommands)
            {
                if (!CpbpParams.Args.Contains(argumentSeperator + command.Key.Name))
                {
                    Console.WriteLine();
                    throw new Exception($"You did not call a required command! Command name : {command.Key.Name}");
                }
            }

            /// execute commands
            foreach (var argument in CpbpParams.Args)
            {
                foreach (var command in orderedCommands)
                {
                    if (argument.Equals(command.Key.Name)) Execute(argument, command.Key, command.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        protected void Execute(string argument, Type applicationType, object commandInstance)
        {
            if (argument.Contains(argumentSeperator))
            {
                CpbpParams.Argument = argument.Replace(argumentSeperator, "");
                CpbpParams.Value = string.Empty;
                CpbpParams.Value = GetArgValue(CpbpParams.Args, CpbpParams.Argument);

                Type applicationServiceType;

                var handler = GetApplicationInstanceAndServiceType(applicationType, out applicationServiceType);

                if (handler != null)
                {
                    var method = handler.GetType().GetMethod(CliAppMethodName);
                    commandInstance.GetType().GetProperty("Value").SetValue(commandInstance, CpbpParams.Value);
                    method.Invoke(handler, new object[] { commandInstance });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="applicationServiceType"></param>
        /// <returns></returns>
        private object GetApplicationInstanceAndServiceType(Type applicationType, out Type applicationServiceType)
        {
            var handlerTypes = GetApplicationHandlerTypes();

            applicationServiceType = null;

            var instanceProducer = CpbpBootstrapper.IocContainer.GetCurrentRegistrations();

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
                            applicationServiceType = serviceItem.ServiceType;
                            return CpbpBootstrapper.IocContainer.GetInstance(serviceItem.ServiceType);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected string GetArgValue(string[] args, string argument)
        {
            try
            {
                return args[args.ToList().FindIndex(x => x.Equals(argument)) + 2];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Type GetTypeByName(string name)
        {
            return (from types in assemblyTypes
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

            assemblyTypes = null;

            CpbpBootstrapper.IocContainer= null;

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        private class DictonaryDto
        {
            public Type Key { get; set; }
            public object Value { get; set; }
        }
    }
}
