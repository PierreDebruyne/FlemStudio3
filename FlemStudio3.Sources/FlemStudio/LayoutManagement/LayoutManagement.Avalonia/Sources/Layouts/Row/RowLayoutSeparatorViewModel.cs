namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class RowLayoutSeparatorViewModel : RowLayoutElementViewModel
    {
#if DEBUG
        public RowLayoutSeparatorViewModel() : base(0)
        {

        }
#endif
        public RowLayoutSeparatorViewModel(int position) : base(position)
        {
        }

        public override void Dispose()
        {

        }
    }
}
