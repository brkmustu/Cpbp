using Cpbp;

namespace WithModuleUsage
{
    /// <summary>
    /// custom application module. this class contain Ioc container.
    /// You can add the services and classes you want to register here by override the "CustomRegistrations" method.
    /// </summary>
    public class SampleModule : CpbpModule
    {
        public override void CustomRegistrations()
        {
            IocContainer.Register<ILogger, Logger>();
        }
    }
}
