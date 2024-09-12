
using FlemStudio.LayoutManagement.Core.Layouts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public abstract class RowLayoutElementViewModel : ReactiveObject
    {
        protected int _Position;
        public int Position
        {
            get => _Position;
            internal set => this.RaiseAndSetIfChanged(ref _Position, value);
        }

        public RowLayoutElementViewModel(int position)
        {
            _Position = position;
        }

        public abstract void Dispose();
    }



    public class RowLayoutViewModel : LayoutViewModel
    {



        protected RowLayoutViewModelType LayoutViewModelType;
        internal RowLayoutUser RowLayout;

     

        public RowLayoutViewModel(LayoutViewModelService layoutViewModelService, RowLayoutViewModelType layoutViewModelType, RowLayoutUser rowLayout) : base(layoutViewModelService, layoutViewModelType)
        {
            LayoutViewModelType = layoutViewModelType;
            RowLayout = rowLayout;
            Init();
        }

        public ObservableCollection<RowLayoutElementViewModel> Elements { get; } = new();


        private void Init()
        {
            RowLayout.ItemAdded += OnItemAdded;
            RowLayout.ItemRemoved += OnItemRemoved;
            RowLayout.ItemLayoutGuidChanged += OnItemLayoutGuidChanged;

            int itemCount = RowLayout.ItemCount;
            int position = 0;
            for (int i = 0; i < itemCount; i++)
            {
                if (i > 0)
                {
                    Elements.Add(new RowLayoutSeparatorViewModel(position));
                    position++;
                }
                Elements.Add(new RowLayoutItemViewModel(position, RowLayout.GetSize(i), this, RowLayout.GetLayoutGuid(i)));
                position++;
            }



        }

        private void OnItemLayoutGuidChanged(int index, Guid oldGuid, Guid newGuid)
        {
            int position = 0;
            for (int i = 0; i < index; i++)
            {

                position += 2;
            }
            RowLayoutItemViewModel item = (RowLayoutItemViewModel)Elements[position];
            item.SetLayoutGuid(newGuid);

        }

        public override void Dispose()
        {
            foreach (var element in Elements)
            {
                element.Dispose();
            }
            RowLayout.Dispose();
        }

        private void OnItemAdded(int index)
        {
            int position = 0;
            if (index == 0)
            {
                Elements.Insert(position, new RowLayoutItemViewModel(position, RowLayout.GetSize(index), this, RowLayout.GetLayoutGuid(index)));
                position++;
                Elements.Insert(position, new RowLayoutSeparatorViewModel(position));
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    if (i > 0)
                    {
                        position++;
                    }
                    position++;
                }
                Elements.Insert(position, new RowLayoutSeparatorViewModel(position));
                position++;
                Elements.Insert(position, new RowLayoutItemViewModel(position, RowLayout.GetSize(index), this, RowLayout.GetLayoutGuid(index)));
                position++;
            }
            while (position < Elements.Count)
            {
                Elements[position].Position = position;
                position++;
            }

            //this.RaisePropertyChanged(nameof(IsTableVisible));
        }

        private void OnItemRemoved(int index, Guid itemGuid)
        {
            int position = 0;
            for (int i = 0; i < index; i++)
            {
                if (i > 0)
                {
                    position++;
                }
                position++;
            }
            RowLayoutElementViewModel item = Elements[position];
            Elements.RemoveAt(position);
            item.Dispose();
            item = Elements[position];
            Elements.RemoveAt(position);
            item.Dispose();

            while (position < Elements.Count)
            {
                Elements[position].Position = position;
                position++;
            }
            if (itemGuid != Guid.Empty)
            {
                LayoutViewModelService.RemoveLayout(itemGuid, true);
            }
            if (RowLayout.ItemCount == 1)
            {
                this.NeedSimplify?.Invoke(RowLayout.GetLayoutGuid(0));
            }

        }

        internal void OnItemSizeChanged(int index, float size)
        {
            RowLayout.ResizeItem(index, size);
        }

        internal void OnItemNeedSimplify(int index, Guid layoutGuidToKeep)
        {
            LayoutUser layoutToKeep = LayoutViewModelService.UseLayout(layoutGuidToKeep);

            Guid oldItemGuid = RowLayout.GetLayoutGuid(index);
            RowLayout.SetLayoutGuid(index, layoutGuidToKeep);
            LayoutViewModelService.RemoveLayout(oldItemGuid, false);

            layoutToKeep.Dispose();
        }

        internal void AddItemCommand(int index, List<object> parameters)
        {
            LayoutUser layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            RowLayout.AddItem(index, layout.LayoutGuid);
            layout.Dispose();
        }

        public void AddItemLeft(List<object> parameters)
        {
            AddItemCommand(0, parameters);
        }

        public void AddItemRight(List<object> parameters)
        {
            AddItemCommand(RowLayout.ItemCount, parameters);
        }



        internal void OnItemRemoveCommand(int index)
        {
            RowLayout.RemoveItem(index);
        }

        internal void OnItemAddLeftCommand(int index, List<object> parameters)
        {
            AddItemCommand(index, parameters);
        }

        internal void OnItemAddRightCommand(int index, List<object> parameters)
        {
            AddItemCommand(index + 1, parameters);
        }

        internal void OnItemSplitTopCommand(int index, List<object> parameters)
        {
            ColumnLayoutUser columnlayout = (ColumnLayoutUser)LayoutViewModelService.UseNewLayout("Column");

            LayoutUser item0Layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            columnlayout.SetLayoutGuid(0, item0Layout.LayoutGuid);
            item0Layout.Dispose();

            columnlayout.SetLayoutGuid(1, RowLayout.GetLayoutGuid(index));
            RowLayout.SetLayoutGuid(index, columnlayout.LayoutGuid);

            columnlayout.Dispose();
        }

        internal void OnItemSplitBottomCommand(int index, List<object> parameters)
        {
            ColumnLayoutUser columnlayout = (ColumnLayoutUser)LayoutViewModelService.UseNewLayout("Column");

            LayoutUser item1Layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            columnlayout.SetLayoutGuid(1, item1Layout.LayoutGuid);
            item1Layout.Dispose();

            columnlayout.SetLayoutGuid(0, RowLayout.GetLayoutGuid(index));
            RowLayout.SetLayoutGuid(index, columnlayout.LayoutGuid);



            columnlayout.Dispose();
        }

        internal void OnItemError(int index)
        {
            LayoutUser newLayout = LayoutViewModelService.UseNewLayout("Tab");
            RowLayout.SetLayoutGuid(index, newLayout.LayoutGuid);
            newLayout.Dispose();
        }
    }
}
