
using ReactiveUI;
using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class RowLayoutItemViewModel : RowLayoutElementViewModel
    {

#if DEBUG
        public RowLayoutItemViewModel() : base(0)
        {

        }
#endif

        public LayoutViewModel? LayoutViewModel { get; protected set; }
        protected RowLayoutViewModel RowLayoutViewModel;

        protected float _Size;
        public float Size
        {
            get => _Size;
            set
            {
                this.RaiseAndSetIfChanged(ref _Size, value);
            }
        }

        public bool EditMode => RowLayoutViewModel.LayoutViewModelService.EditMode;
        private void OnEditModeChanged(bool editMode)
        {
            this.RaisePropertyChanged(nameof(EditMode));
        }
        public RowLayoutItemViewModel(int position, float size, RowLayoutViewModel rowLayoutViewModel, Guid layoutGuid) : base(position)
        {
            RowLayoutViewModel = rowLayoutViewModel;
            _Size = size;

            RowLayoutViewModel.LayoutViewModelService.EditModeChanged += OnEditModeChanged;

            if (layoutGuid != Guid.Empty)
            {
                try
                {
                    LayoutViewModel = RowLayoutViewModel.LayoutViewModelService.CreateLayoutViewModel(layoutGuid);
                    LayoutViewModel.NeedSimplify += OnNeedSimplify;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    RowLayoutViewModel.OnItemError(Position / 2);
                }

            }

            AddLeftMenuViewModel = new MenuViewModel(RowLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            AddLeftMenuViewModel.Submit = AddLeftCommand;
            AddRightMenuViewModel = new MenuViewModel(RowLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            AddRightMenuViewModel.Submit = AddRightCommand;
            SplitTopMenuViewModel = new MenuViewModel(RowLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            SplitTopMenuViewModel.Submit = SplitTopCommand;
            SplitBottomMenuViewModel = new MenuViewModel(RowLayoutViewModel.LayoutViewModelService.CreateContainerMenuItemViewModels());
            SplitBottomMenuViewModel.Submit = SplitBottomCommand;
        }

        public MenuViewModel AddLeftMenuViewModel { get; protected set; }
        public MenuViewModel AddRightMenuViewModel { get; protected set; }
        public MenuViewModel SplitTopMenuViewModel { get; protected set; }
        public MenuViewModel SplitBottomMenuViewModel { get; protected set; }

        public override void Dispose()
        {
            RowLayoutViewModel.LayoutViewModelService.EditModeChanged -= OnEditModeChanged;
        }

        public void OnSizeChanged(float value)
        {
            Size = value;
            //Debug.WriteLine("Item " + Position + " size changed: " + Size);
            RowLayoutViewModel.OnItemSizeChanged(Position / 2, Size);
        }

        public void OnNeedSimplify(Guid layoutToKeep)
        {
            RowLayoutViewModel.OnItemNeedSimplify(Position / 2, layoutToKeep);
        }

        public void RemoveItemCommand()
        {
            RowLayoutViewModel.OnItemRemoveCommand(Position / 2);
        }

        public void AddLeftCommand(List<object> parameters)
        {
            RowLayoutViewModel.OnItemAddLeftCommand(Position / 2, parameters);
        }

        public void AddRightCommand(List<object> parameters)
        {
            RowLayoutViewModel.OnItemAddRightCommand(Position / 2, parameters);
        }

        public bool CanSplit => LayoutViewModel?.LayoutType.Name == "ApplicationTable" || LayoutViewModel?.LayoutType.Name == "EditorTab";

        public void SplitTopCommand(List<object> parameters)
        {
            if (!CanSplit) return;

            RowLayoutViewModel.OnItemSplitTopCommand(Position / 2, parameters);
        }

        public void SplitBottomCommand(List<object> parameters)
        {
            if (!CanSplit) return;

            RowLayoutViewModel.OnItemSplitBottomCommand(Position / 2, parameters);
        }

        internal void SetLayoutGuid(Guid newGuid)
        {

            LayoutViewModel?.Dispose();
            LayoutViewModel = null;
            if (newGuid != Guid.Empty)
            {
                try
                {
                    LayoutViewModel = RowLayoutViewModel.LayoutViewModelService.CreateLayoutViewModel(newGuid);
                    LayoutViewModel.NeedSimplify += OnNeedSimplify;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    RowLayoutViewModel.OnItemError(Position / 2);
                }

            }
            this.RaisePropertyChanged(nameof(LayoutViewModel));
            this.RaisePropertyChanged(nameof(CanSplit));
        }
    }
}
