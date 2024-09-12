
namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class ColumnLayoutSeparatorViewModel : ColumnLayoutElementViewModel
    {
#if DEBUG
        public ColumnLayoutSeparatorViewModel() : base(0)
        {

        }
#endif
        public ColumnLayoutSeparatorViewModel(int position) : base(position)
        {
        }

        public override void Dispose()
        {

        }
    }
}
