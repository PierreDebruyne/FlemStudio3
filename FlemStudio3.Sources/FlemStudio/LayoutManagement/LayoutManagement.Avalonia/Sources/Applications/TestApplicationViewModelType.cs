using FlemStudio.LayoutManagement.Core.Applications;

namespace FlemStudio.LayoutManagement.Avalonia.Applications
{
    public class TestApplicationViewModelType : ApplicationViewModelType<TestApplicationType, TestApplicationViewModel, TestApplicationUser>
    {
        public TestApplicationViewModelType(TestApplicationType type) : base(type)
        {
        }

        public override string Name => "Test Application";



        public override TestApplicationViewModel DoCreateApplicationViewModel(TestApplicationUser user)
        {
            return new TestApplicationViewModel(this, user);
        }
    }
}
