using Avalonia.Controls;
using Avalonia.Input;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public partial class AssetDirectoryItemView : UserControl
    {
        public AssetDirectoryItemView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            StackPanel? container = this.FindControl<StackPanel>("Container");
            if (container != null)
            {
                container.PointerPressed += OnPointerPressed;
            }
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // handle double-click 
            if (e.ClickCount == 2)
            {

                AssetDirectoryItemViewModel? model = (AssetDirectoryItemViewModel?)this.DataContext;
                model?.Open();
            }
        }
    }
}
