using FlemStudio.LayoutManagement.Core.Layouts;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{


    public class ColumnLayoutViewModelType : LayoutViewModelType
    {
        public ColumnLayoutViewModelType(LayoutType type) : base(type)
        {

        }

        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new ColumnLayoutViewModel(layoutViewModelService, this, (ColumnLayoutUser)user);
        }
    }
}
