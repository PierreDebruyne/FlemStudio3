using ReactiveUI;

namespace FlemStudio.LayoutManagement.Avalonia.Applications
{


    public class EmptyApplicationViewModel : ReactiveObject, IApplicationViewModel
    {


        public string Header => "Empty application";

        public string TabItem => "Empty";

        public void Dispose()
        {
        }
    }
}
