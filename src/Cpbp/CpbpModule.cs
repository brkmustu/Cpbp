using Cpbp.Core;
using SimpleInjector;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Cpbp
{
    public class CpbpModule
    {
        /// <summary>
        /// is the suffix name of the types of Cpbp applications to be searched for.
        /// </summary>
        private const string CpbpApplicationRootName = "Application";

        /// <summary>
        ///  is the suffix name of the types of Cpbp application handlers to be searched for.
        /// </summary>
        private const string CpbpApplicationHandlerRootName = "ApplicationHandler";


        private Assembly[] _assemblies = null;
        private string _argumentSeperator = "-";
        private Container _container = null;
        private string _applicationName = string.Empty;
        private Dictionary<string, string> _applicationParameters = null;
        private object _applicationInstance = null;
        private object _applicationHandlerInstance = null;

        public Assembly[] Assemblies => _assemblies;
        public Dictionary<string, string> ApplicationParameters => _applicationParameters;
        public string ArgumentSeperator => _argumentSeperator;
        public Container IocContainer => _container;

        /// <summary>
        /// method that express mandatory parameters
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="args"></param>
        public void Requirements(Assembly[] assemblies, string[] args)
        {
            if (args == null || args.Length == 0) throw new ArgumentNullException("The application cannot be run without the application parameter!");
            _assemblies = assemblies;
            _applicationName = args[0];
            SetArguments(args);
            var _applicationType = GetApplicationType(_applicationName);
            _applicationInstance = Activator.CreateInstance(_applicationType);
        }

        public void WithArgumentSeperator([MaxLength(2)]string seperator)
        {
            _argumentSeperator = seperator;
        }

        public void WithContainer(Container container)
        {
            _container = container;
        }

        public CpbpModule SetOptions(Action<CpbpModule> factory)
        {
            factory.Invoke(this);

            if (_assemblies == null || _assemblies.Length.Equals(0)) throw new ArgumentNullException($"At least one assembly assignment must be made using the 'WithAssemblies' method.");

            RegisterCpbpDependecies();

            _applicationHandlerInstance = GetApplicationHandlerInstance(_applicationInstance.GetType());

            return this;
        }

        public CpbpProgram GetProgram()
        {
            return new CpbpProgram(_applicationHandlerInstance, _applicationInstance);
        }

        private void RegisterCpbpDependecies()
        {
            if (_container == null) _container = new Container();

            _container.Register(typeof(ICpbpApplicationHandler<>), _assemblies);

            _container.RegisterDecorator(typeof(ICpbpApplicationHandler<>), typeof(CpbpApplicationHandlerDecorator<>));

            _container.Verify();
        }

        /// <summary>
        /// Separate parameters and application partitions.
        /// </summary>
        /// <param name="args"></param>
        private void SetArguments(string[] args)
        {
            if (args.Length > 1) _applicationParameters = new Dictionary<string, string>();

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].Contains(_argumentSeperator))
                {
                    if (args.Length <= i + 1)
                    {
                        throw new NullReferenceException("Invalid application parameter.");
                    }

                    _applicationParameters.Add(args[i], args[i + 1]);
                }
            }
        }

        /// <summary>
        /// Returns the instance of the given cpbp application
        /// </summary>
        /// <param name="applicationType">Type inherited from "CpbpApplication". Not handler type</param>
        /// <returns>application handler instance</returns>
        private object GetApplicationHandlerInstance(Type applicationType)
        {
            var handlerTypes = GetApplicationHandlerTypes();

            var instanceProducer = _container.GetCurrentRegistrations();

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
                            return _container.GetInstance(serviceItem.ServiceType);
                        }
                    }
                }
            }

            return null;
        }

        protected Type GetApplicationType(string applicationName) =>
            (
                from type in _assemblies.SelectMany(x => x.GetExportedTypes()).ToList()
                where type.Name.Equals(applicationName)
                select type
            ).FirstOrDefault();

        protected IEnumerable<Type> GetApplicationHandlerTypes() =>
            from type in _assemblies.SelectMany(x => x.GetExportedTypes()).ToList()
            where type.Name.EndsWith(CpbpApplicationHandlerRootName)
            select type;
    }
}
