using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetManagement.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.Avalonia
{
    public class CreateAssetDirectoryDialogViewModel : DialogViewModel
    {
        public override string DialogTitle => "Create Asset Directory";
        public override double MinHeight => 150;
        public override double MinWidth => 400;

        protected IAssetContainer ParentContainer;

        public CreateAssetDirectoryDialogViewModel(IAssetContainer parentContainer)
        {
            ParentContainer = parentContainer;
        }

        private string _Name = "";
        public string Name
        {
            get => _Name;
            set
            {
                this.RaiseAndSetIfChanged(ref _Name, value);
                this.RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        public string ParentFolderPath => ParentContainer.Info.AssetPath;

        public void Submit()
        {
            this.OnSubmit?.Invoke();
        }

        public void Cancel()
        {
            this.OnCancel?.Invoke();
        }

        public bool CanSubmit => Name.Length > 0;
    }
}
