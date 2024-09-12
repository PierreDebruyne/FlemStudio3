using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public partial class ColumnLayoutView : UserControl
    {
        public ColumnLayoutView()
        {
            InitializeComponent();
        }

        protected ColumnLayoutViewModel? ViewModel;
        protected Grid Grid;


        protected override void OnInitialized()
        {
            base.OnInitialized();


            ViewModel = (ColumnLayoutViewModel?)this.DataContext;
            if (ViewModel != null)
            {
                ViewModel.Elements.CollectionChanged += Test;
            }
            ItemsControlPanel.Loaded += this.ItemsControlLoaded;

        }

        private void ItemsControlLoaded(object? sender, RoutedEventArgs e)
        {
            Grid = (Grid)ItemsControlPanel.ItemsPanelRoot;

            RecomputeSizes();


        }

        protected void RecomputeSizes()
        {
            Grid.RowDefinitions.Clear();
            foreach (var item in ViewModel.Elements)
            {
                if (item.GetType().IsAssignableTo(typeof(ColumnLayoutSeparatorViewModel)))
                {
                    Grid.RowDefinitions.Add(new RowDefinition(3, GridUnitType.Pixel));
                }
                else
                {
                    ColumnLayoutItemViewModel listItem = (ColumnLayoutItemViewModel)item;
                    RowDefinition rowDef = new RowDefinition(listItem.Size, GridUnitType.Star);
                    rowDef.PropertyChanged += (object? sender, AvaloniaPropertyChangedEventArgs e) =>
                    {
                        if (e.Property == RowDefinition.HeightProperty)
                        {
                            NotifySizeChanged();
                        }
                    };
                    Grid.RowDefinitions.Add(rowDef);
                }

            }
            //Grid.InvalidateArrange();
            //NotifySizeChanged();
        }

        protected void NotifySizeChanged()
        {
            double fullSize = GetFullSize();
            for (int i = 0; i < ViewModel.Elements.Count; i++)
            {
                if (ViewModel.Elements[i].GetType().IsAssignableTo(typeof(ColumnLayoutItemViewModel)))
                {
                    ColumnLayoutItemViewModel listItem = (ColumnLayoutItemViewModel)ViewModel.Elements[i];
                    RowDefinition rowDef = Grid.RowDefinitions[i];
                    GridLength height = rowDef.GetValue<GridLength>(RowDefinition.HeightProperty);
                    listItem.OnSizeChanged((float)(height.Value / fullSize));
                }
            }

        }

        protected double GetFullSize()
        {
            double size = 0f;

            for (int i = 0; i < ViewModel.Elements.Count; i++)
            {
                if (ViewModel.Elements[i].GetType().IsAssignableTo(typeof(ColumnLayoutItemViewModel)))
                {
                    RowDefinition rowDef = Grid.RowDefinitions[i];
                    size += rowDef.GetValue<GridLength>(RowDefinition.HeightProperty).Value;
                }

            }
            return size;
        }




        private void Test(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine(e.Action);
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RecomputeSizes();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                RecomputeSizes();
            }
        }
    }
}
