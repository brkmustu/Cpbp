using Cpbp.Contracts;

namespace Cpbp.Dependency
{
    /// <summary>
    /// cpbp is the interface to handle application partition codes
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    public interface ICpbpApplicationHandler<TApplication>
        where TApplication : CpbpApplication
    {
        void Handle(TApplication application);
    }
}
