namespace Cpbp.Tests.Models
{
    public class TestApplicationHandler : Core.ICpbpApplicationHandler<TestApplication>
    {
        public void Handle(TestApplication application)
        {
            TestParameters.IsWorked = true;
        }
    }
}
