using Avalonia.Controls;
using Avalonia.Input;
using System.ComponentModel;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public partial class AssetContainerContentItemView : UserControl
    {
        public AssetContainerContentItemView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Container.PointerPressed += OnPointerPressed;

        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // handle double-click 
            AssetContainerContentItemViewModel? model = (AssetContainerContentItemViewModel?)this.DataContext;
            if (model != null)
            {
                
                bool multiple = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
                if (e.ClickCount >= 2 && multiple == false)
                {
                    model?.Open();
                }

                model?.Select(multiple);

            }
            
            
            
        }
    }
}
