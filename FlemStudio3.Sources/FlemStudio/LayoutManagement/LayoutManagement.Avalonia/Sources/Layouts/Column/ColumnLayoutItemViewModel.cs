using ReactiveUI;
using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class ColumnLayoutItemViewModel : ColumnLayoutElementViewModel
    {

#if DEBUG
        public ColumnLayoutItemViewModel() : base(0)
        {

        }
#endif

        public LayoutViewModel? LayoutViewModel { get; protected set; }
        protected ColumnLayoutViewModel ColumnLayoutViewModel;

        protected float _Size;
        public float Size
        {
            get => _Size;
            set
            {
                this.RaiseAndSetIfChanged(ref _Size, value);
            }
        }

        public bool EditMode => ColumnLayoutViewModel.LayoutViewModelService.EditMode;
        private void OnEditModeChanged(bool editMode)
        {
            this.RaisePropertyChanged(nameof(EditMode));
        }
        public ColumnLayoutItemViewModel(int position, float size, ColumnLayoutViewModel columnLayoutViewModel, Guid layoutGuid) : base(position)
        {
            ColumnLayoutViewModel = columnLayoutViewModel;
            _Size = size;

            ColumnLayoutViewModel.LayoutViewModelService.EditModeChanged += OnEditModeChanged;

            if (layoutGuid != Guid.Empty)
            {
                try
                {
                    LayoutViewModel = ColumnLayoutViewModel.LayoutViewModelService.CreateLayoutViewModel(layoutGuid);
                    LayoutViewModel.NeedSimplify += OnNeedSimplify;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    ColumnLayoutViewModel.OnItemError(Position / 2);
                }

            }

            AddTopMenuViewModel = new MenuViewModel(ColumnLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            AddTopMenuViewModel.Submit = AddTopCommand;
            AddBottomMenuViewModel = new MenuViewModel(ColumnLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            AddBottomMenuViewModel.Submit = AddBottomCommand;
            SplitLeftMenuViewModel = new MenuViewModel(ColumnLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            SplitLeftMenuViewModel.Submit = SplitLeftCommand;
            SplitRightMenuViewModel = new MenuViewModel(ColumnLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            SplitRightMenuViewModel.Submit = SplitRightCommand;

        }


        public MenuViewModel AddTopMenuViewModel { get; protected set; }
        public MenuViewModel AddBottomMenuViewModel { get; protected set; }
        public MenuViewModel SplitLeftMenuViewModel { get; protected set; }
        public MenuViewModel SplitRightMenuViewModel { get; protected set; }


        public override void Dispose()
        {
            ColumnLayoutViewModel.LayoutViewModelService.EditModeChanged -= OnEditModeChanged;
        }

        public void OnSizeChanged(float value)
        {
            Size = value;
            //Debug.WriteLine("Item " + Position + " size changed: " + Size);
            ColumnLayoutViewModel.OnItemSizeChanged(Position / 2, Size);
        }

        public void OnNeedSimplify(Guid layoutToKeep)
        {
            ColumnLayoutViewModel.OnItemNeedSimplify(Position / 2, layoutToKeep);
        }

        public void RemoveItemCommand()
        {
            ColumnLayoutViewModel.OnItemRemoveCommand(Position / 2);
        }

        public void AddTopCommand(List<object> parameters)
        {
            ColumnLayoutViewModel.OnItemAddTopCommand(Position / 2, parameters);
        }

        public void AddBottomCommand(List<object> parameters)
        {
            ColumnLayoutViewModel.OnItemAddBottomCommand(Position / 2, parameters);
        }

        public bool CanSplit => LayoutViewModel?.LayoutType.Name == "ApplicationTable" || LayoutViewModel?.LayoutType.Name == "EditorTab";

        public void SplitLeftCommand(List<object> parameters)
        {
            if (!CanSplit) return;

            ColumnLayoutViewModel.OnItemSplitLeftCommand(Position / 2, parameters);
        }

        public void SplitRightCommand(List<object> parameters)
        {
            if (!CanSplit) return;

            ColumnLayoutViewModel.OnItemSplitRightCommand(Position / 2, parameters);
        }

        internal void SetLayoutGuid(Guid newGuid)
        {
            LayoutViewModel?.Dispose();
            LayoutViewModel = null;
            if (newGuid != Guid.Empty)
            {
                try
                {
                    LayoutViewModel = ColumnLayoutViewModel.LayoutViewModelService.CreateLayoutViewModel(newGuid);
                    LayoutViewModel.NeedSimplify += OnNeedSimplify;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    ColumnLayoutViewModel.OnItemError(Position / 2);
                }

            }
            this.RaisePropertyChanged(nameof(LayoutViewModel));
            this.RaisePropertyChanged(nameof(CanSplit));
        }
    }
}
