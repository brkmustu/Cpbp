namespace Cpbp.Core
{
    /// <summary>
    /// cpbp is the interface to handle application partition codes
    /// </summary>
    /// <typeparam name="TApplication">application type</typeparam>
    public interface ICpbpApplicationHandler<TApplication>
        where TApplication : CpbpApplicationBase
    {
        void Handle(TApplication application);
    }
}
