
using FlemStudio.LayoutManagement.Core.Layouts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public abstract class ColumnLayoutElementViewModel : ReactiveObject
    {
        protected int _Position;
        public int Position
        {
            get => _Position;
            internal set => this.RaiseAndSetIfChanged(ref _Position, value);
        }

        public ColumnLayoutElementViewModel(int position)
        {
            _Position = position;
        }

        public abstract void Dispose();
    }


    public class ColumnLayoutViewModel : LayoutViewModel
    {


        protected ColumnLayoutViewModelType LayoutViewModelType;

        internal ColumnLayoutUser ColumnLayout;



        public ColumnLayoutViewModel(LayoutViewModelService layoutViewModelService, ColumnLayoutViewModelType layoutViewModelType, ColumnLayoutUser columnLayout) : base(layoutViewModelService,layoutViewModelType)
        {
            LayoutViewModelType = layoutViewModelType;
            ColumnLayout = columnLayout;
            Init();
        }

        public ObservableCollection<ColumnLayoutElementViewModel> Elements { get; } = new();


        private void Init()
        {


            ColumnLayout.ItemAdded += OnItemAdded;
            ColumnLayout.ItemRemoved += OnItemRemoved;
            ColumnLayout.ItemLayoutGuidChanged += OnItemLayoutGuidChanged;

            int itemCount = ColumnLayout.ItemCount;
            int position = 0;
            for (int i = 0; i < itemCount; i++)
            {
                if (i > 0)
                {
                    Elements.Add(new ColumnLayoutSeparatorViewModel(position));
                    position++;
                }
                Elements.Add(new ColumnLayoutItemViewModel(position, ColumnLayout.GetSize(i), this, ColumnLayout.GetLayoutGuid(i)));
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
            ColumnLayoutItemViewModel item = (ColumnLayoutItemViewModel)Elements[position];
            item.SetLayoutGuid(newGuid);

        }

        public override void Dispose()
        {
            foreach (var element in Elements)
            {
                element.Dispose();
            }
            ColumnLayout.Dispose();
        }

        private void OnItemAdded(int index)
        {
            int position = 0;
            if (index == 0)
            {
                Elements.Insert(position, new ColumnLayoutItemViewModel(position, ColumnLayout.GetSize(index), this, ColumnLayout.GetLayoutGuid(index)));
                position++;
                Elements.Insert(position, new ColumnLayoutSeparatorViewModel(position));
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
                Elements.Insert(position, new ColumnLayoutSeparatorViewModel(position));
                position++;
                Elements.Insert(position, new ColumnLayoutItemViewModel(position, ColumnLayout.GetSize(index), this, ColumnLayout.GetLayoutGuid(index)));
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
            ColumnLayoutElementViewModel item = Elements[position];
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

            if (ColumnLayout.ItemCount == 1)
            {
                this.NeedSimplify?.Invoke(ColumnLayout.GetLayoutGuid(0));
            }

        }

        internal void OnItemSizeChanged(int index, float size)
        {
            ColumnLayout.ResizeItem(index, size);
        }

        internal void OnItemNeedSimplify(int index, Guid layoutGuidToKeep)
        {
            LayoutUser layoutToKeep = LayoutViewModelService.UseLayout(layoutGuidToKeep);

            Guid oldItemGuid = ColumnLayout.GetLayoutGuid(index);
            ColumnLayout.SetLayoutGuid(index, layoutGuidToKeep);
            LayoutViewModelService.RemoveLayout(oldItemGuid, false);

            layoutToKeep.Dispose();
        }

        internal void AddItemCommand(int index, List<object> parameters)
        {
            LayoutUser layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            ColumnLayout.AddItem(index, layout.LayoutGuid);
            layout.Dispose();
        }

        public void AddItemTop(List<object> parameters)
        {
            AddItemCommand(0, parameters);
        }

        public void AddItemBottom(List<object> parameters)
        {
            AddItemCommand(ColumnLayout.ItemCount, parameters);
        }



        internal void OnItemRemoveCommand(int index)
        {
            ColumnLayout.RemoveItem(index);
        }

        internal void OnItemAddTopCommand(int index, List<object> parameters)
        {
            AddItemCommand(index, parameters);
        }

        internal void OnItemAddBottomCommand(int index, List<object> parameters)
        {
            AddItemCommand(index + 1, parameters);
        }

        internal void OnItemSplitLeftCommand(int index, List<object> parameters)
        {
            RowLayoutUser rowlayout = (RowLayoutUser)LayoutViewModelService.UseNewLayout("Row");

            LayoutUser item0Layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            rowlayout.SetLayoutGuid(0, item0Layout.LayoutGuid);
            item0Layout.Dispose();

            rowlayout.SetLayoutGuid(1, ColumnLayout.GetLayoutGuid(index));
            ColumnLayout.SetLayoutGuid(index, rowlayout.LayoutGuid);

            rowlayout.Dispose();
        }

        internal void OnItemSplitRightCommand(int index, List<object> parameters)
        {
            RowLayoutUser rowlayout = (RowLayoutUser)LayoutViewModelService.UseNewLayout("Row");

            LayoutUser item1Layout = LayoutViewModelService.UseNewLayout((string)parameters[0]);
            rowlayout.SetLayoutGuid(1, item1Layout.LayoutGuid);
            item1Layout.Dispose();

            rowlayout.SetLayoutGuid(0, ColumnLayout.GetLayoutGuid(index));
            ColumnLayout.SetLayoutGuid(index, rowlayout.LayoutGuid);



            rowlayout.Dispose();
        }

        internal void OnItemError(int index)
        {
            LayoutUser newLayout = LayoutViewModelService.UseNewLayout("Tab");
            ColumnLayout.SetLayoutGuid(index, newLayout.LayoutGuid);
            newLayout.Dispose();
        }
    }
}
