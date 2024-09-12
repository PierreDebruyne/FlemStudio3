using Avalonia.Controls;
using Avalonia.Input;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public partial class ApplicationTableLayoutItemView : UserControl
    {
        public ApplicationTableLayoutItemView()
        {
            InitializeComponent();
        }

        ApplicationTableLayoutItemViewModel ViewModel;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ViewModel = (ApplicationTableLayoutItemViewModel)this.DataContext;

            TabItem.PointerPressed += OnClick;
        }



        private void OnClick(object? sender, PointerPressedEventArgs e)
        {
            (this.DataContext as ApplicationTableLayoutItemViewModel).OnClickTab();
        }
    }
}
