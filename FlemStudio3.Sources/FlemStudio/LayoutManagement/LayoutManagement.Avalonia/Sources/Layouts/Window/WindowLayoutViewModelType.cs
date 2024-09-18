using FlemStudio.LayoutManagement.Core.Layouts;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class WindowLayoutViewModelType : LayoutViewModelType
    {
        public WindowLayoutViewModelType(LayoutType layoutType) : base(layoutType)
        {
        }

        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new WindowLayoutViewModel(layoutViewModelService, this, (WindowLayoutUser)user);
        }
    }
}
