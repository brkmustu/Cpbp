using Cpbp.Contracts;

namespace Cpbp.Dependency
{
    public interface ICpbpApplicationHandler<TApplication>
        where TApplication : CpbpApplication
    {
        void Handle(TApplication application);
    }
}
