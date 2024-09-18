using FlemStudio.AssetManagement.Core.AssetDirectories;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetDirectoryItemViewModel : AssetContainerContentItemContentViewModel
    {
        internal AssetDirectory AssetDirectory;

        public AssetDirectoryItemViewModel(AssetDirectory assetDirectory)
        {
            AssetDirectory = assetDirectory;
        }

        public string Name => AssetDirectory.Info.Name;

        public Action<AssetDirectory>? OnOpen;
        public Action<AssetDirectory>? OnRemove;
        public override void Open()
        {
            OnOpen?.Invoke(AssetDirectory);
        }

        public override void Rename()
        {
            
        }
        public override void Remove()
        {
            OnRemove?.Invoke(AssetDirectory);
        }

        public override void Dispose()
        {
            OnOpen = null;
        }

        

        
    }
}
