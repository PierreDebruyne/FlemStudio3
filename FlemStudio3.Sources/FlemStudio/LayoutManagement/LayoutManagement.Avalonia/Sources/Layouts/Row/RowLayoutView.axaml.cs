using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public partial class RowLayoutView : UserControl
    {
        public RowLayoutView()
        {
            InitializeComponent();

        }

        protected RowLayoutViewModel? ViewModel;
        protected Grid Grid;


        protected override void OnInitialized()
        {
            base.OnInitialized();


            ViewModel = (RowLayoutViewModel?)this.DataContext;
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
            Grid.ColumnDefinitions.Clear();
            foreach (var item in ViewModel.Elements)
            {
                if (item.GetType().IsAssignableTo(typeof(RowLayoutSeparatorViewModel)))
                {
                    Grid.ColumnDefinitions.Add(new ColumnDefinition(3, GridUnitType.Pixel));
                }
                else
                {
                    RowLayoutItemViewModel listItem = (RowLayoutItemViewModel)item;
                    ColumnDefinition colDef = new ColumnDefinition(listItem.Size, GridUnitType.Star);
                    colDef.PropertyChanged += (object? sender, AvaloniaPropertyChangedEventArgs e) =>
                    {
                        if (e.Property == ColumnDefinition.WidthProperty)
                        {
                            NotifySizeChanged();
                        }
                    };
                    Grid.ColumnDefinitions.Add(colDef);
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
                if (ViewModel.Elements[i].GetType().IsAssignableTo(typeof(RowLayoutItemViewModel)))
                {
                    RowLayoutItemViewModel listItem = (RowLayoutItemViewModel)ViewModel.Elements[i];
                    ColumnDefinition colDef = Grid.ColumnDefinitions[i];
                    GridLength height = colDef.GetValue<GridLength>(ColumnDefinition.WidthProperty);
                    listItem.OnSizeChanged((float)(height.Value / fullSize));
                }
            }

        }

        protected double GetFullSize()
        {
            double size = 0f;

            for (int i = 0; i < ViewModel.Elements.Count; i++)
            {
                if (ViewModel.Elements[i].GetType().IsAssignableTo(typeof(RowLayoutItemViewModel)))
                {
                    ColumnDefinition colDef = Grid.ColumnDefinitions[i];
                    size += colDef.GetValue<GridLength>(ColumnDefinition.WidthProperty).Value;
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
