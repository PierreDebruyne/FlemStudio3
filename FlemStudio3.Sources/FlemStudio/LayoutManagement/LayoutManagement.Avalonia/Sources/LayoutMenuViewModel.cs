using FlemStudio.LayoutManagement.Avalonia.Layouts;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Avalonia
{
    public class LayoutMenuViewModel : ReactiveObject
    {
        protected LayoutViewModelService LayoutFeatures;

        public List<LayoutMenuItemViewModel> Items { get; } = new();

        public LayoutMenuViewModel(LayoutViewModelService layoutFeatures)
        {
            LayoutFeatures = layoutFeatures;

            Items.Add(new EditModeLayoutMenuItemViewModel(LayoutFeatures));
            Items.Add(new TestLayoutMenuItemViewModel(LayoutFeatures));
        }

        public void Dispose()
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }
        }
    }

    public abstract class LayoutMenuItemViewModel : ReactiveObject
    {
        private string _Header;
        public string Header
        {
            get => _Header;
            set => this.RaiseAndSetIfChanged(ref _Header, value);
        }
        private string? _Icon;
        public string? Icon
        {
            get => _Icon;
            set => this.RaiseAndSetIfChanged(ref _Icon, value);
        }

        public LayoutMenuItemViewModel(string header, string? icon)
        {
            _Header = header;
            _Icon = icon;
        }

        public abstract void Dispose();

        public abstract void Command();
    }

    public class EditModeLayoutMenuItemViewModel : LayoutMenuItemViewModel
    {
        protected LayoutViewModelService LayoutFeatures;
        public EditModeLayoutMenuItemViewModel(LayoutViewModelService layoutFeatures) : base("Edit mode", null)
        {
            LayoutFeatures = layoutFeatures;
            Init();
        }

        public void Init()
        {
            LayoutFeatures.EditModeChanged += OnEditModeChanged;
            Update();
        }

        protected void Update()
        {
            if (LayoutFeatures.EditMode == true)
            {
                Header = "Disable _edit mode";
            }
            else
            {
                Header = "Enable _edit mode";
            }
        }

        private void OnEditModeChanged(bool obj)
        {
            Update();
        }

        public override void Dispose()
        {
            LayoutFeatures.EditModeChanged -= OnEditModeChanged;
        }

        public override void Command()
        {
            LayoutFeatures.EditMode = !LayoutFeatures.EditMode;
        }
    }

    public class TestLayoutMenuItemViewModel : LayoutMenuItemViewModel
    {
        protected LayoutViewModelService LayoutFeatures;
        public TestLayoutMenuItemViewModel(LayoutViewModelService layoutFeatures) : base("_Test", null)
        {
            LayoutFeatures = layoutFeatures;
            Init();
        }

        public void Init()
        {
            
            
        }

        

        

        public override void Dispose()
        {
            
        }

        public override void Command()
        {
            Debug.WriteLine("Test");
        }
    }
}
