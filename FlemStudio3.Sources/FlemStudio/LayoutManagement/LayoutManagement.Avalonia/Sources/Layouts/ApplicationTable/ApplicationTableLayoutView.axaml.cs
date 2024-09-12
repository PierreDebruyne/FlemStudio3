using Avalonia.Controls;
using Avalonia.Input;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public partial class ApplicationTableLayoutView : UserControl
    {
        public ApplicationTableLayoutView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();


            AppContainer.PointerPressed += OnClick;
        }



        private void OnClick(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                (this.DataContext as ApplicationTableLayoutViewModel).Focus();
            }
        }
    }
}
