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
    public class CreateAssetDialogViewModel : DialogViewModel
    {
        public override string DialogTitle => "Create Asset";

        public override double MinHeight => 450;

        public override double MinWidth => 800;


        private string _Name = "ssss";
        public string Name
        {
            get => _Name;
            set
            {
                this.RaiseAndSetIfChanged(ref _Name, value);
            }
        }

        protected AssetManager AssetManager;

        public List<AssetType> AssetTypes { get; }

        public CreateAssetDialogViewModel(AssetManager assetManager)
        {
            AssetManager = assetManager;
            AssetTypes = AssetManager.EnumerateAssetTypes().ToList();
            if (AssetTypes.Count > 0)
            {
                SelectedAssetType = AssetTypes[0];
            }
        }

        protected AssetType? _SelectedAssetType;
        public AssetType? SelectedAssetType
        {
            get => _SelectedAssetType;
            set
            {
                this.RaiseAndSetIfChanged(ref _SelectedAssetType, value);
                

            }
        }

        public void Submit()
        {
            this.OnSubmit?.Invoke();
        }

        public void Cancel()
        {
            this.OnCancel?.Invoke();
        }

        public bool CanSubmit => Name.Length > 0 && SelectedAssetType != null;
    }
}
