using FlemStudio.LayoutManagement.Core.Layouts;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class RowLayoutViewModelType : LayoutViewModelType
    {

        public RowLayoutViewModelType(LayoutType type) : base(type)
        {
        }


        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new RowLayoutViewModel(layoutViewModelService, this, (RowLayoutUser)user);
        }
    }
}
