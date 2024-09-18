using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public partial class AssetContainerContentView : UserControl
    {
        public AssetContainerContentView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            ItemsControl.Loaded += OnLoaded;

        }

   
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            ItemsControl.PointerPressed += OnPointerPressed;
            
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // handle double-click

            
            
            if (e.Source != ItemsControl.ItemsPanelRoot)
            {
                
                return;
            }
            
            if (e.ClickCount == 1)
            {
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    return;
                }
                AssetContainerContentViewModel? model = (AssetContainerContentViewModel?)this.DataContext;
                
                model?.ClearSelection();
            }
           
        }
    }
}
