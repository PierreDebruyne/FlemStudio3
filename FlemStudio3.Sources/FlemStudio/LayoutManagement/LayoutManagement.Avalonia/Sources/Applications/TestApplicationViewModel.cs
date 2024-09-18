using FlemStudio.LayoutManagement.Core.Applications;
using ReactiveUI;

namespace FlemStudio.LayoutManagement.Avalonia.Applications
{
    public class TestApplicationViewModel : ApplicationViewModel<TestApplicationViewModelType, TestApplicationUser>
    {

        public TestApplicationViewModel(TestApplicationViewModelType applicationViewModelType, TestApplicationUser applicationUser) : base(applicationViewModelType, applicationUser)
        {
            ApplicationUser.OnInputUpdated += (string oldValue, string newValue) =>
            {
                this.RaisePropertyChanged(nameof(Input));
            };
        }




        public override string Header => "Test";

        public override string TabItem => "Test";

        public override void Dispose()
        {
            base.Dispose();
        }

        public string Input
        {
            get => ApplicationUser.Input;
            set => ApplicationUser.Input = value;
        }
    }
}
