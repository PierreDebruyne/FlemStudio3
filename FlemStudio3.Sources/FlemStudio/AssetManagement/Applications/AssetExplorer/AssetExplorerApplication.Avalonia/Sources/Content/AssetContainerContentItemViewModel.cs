using FlemStudio.Applications.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{

    public abstract class AssetContainerContentItemContentViewModel : ViewModelBase
    {
        public abstract void Open();
        public abstract void Rename();
        public abstract void Remove();
        public abstract void Dispose();
    }

    public class AssetContainerContentItemViewModel : ViewModelBase
    {

        public AssetContainerContentItemContentViewModel Content { get; }

        public AssetContainerContentItemViewModel(AssetContainerContentItemContentViewModel content)
        {
            Content = content;
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref _IsSelected, value);
            }
        }
        public void Dispose() {
            Content.Dispose();
        }

        public Action<AssetContainerContentItemViewModel, bool>? OnSelect;
        public void Select(bool multiple = false)
        {
            OnSelect?.Invoke(this, multiple);
        }

        public void Open()
        {
            Content.Open();
        }

        public void Rename()
        {

        }

        public void Remove()
        {
            Content.Remove();
        }


    }
}
