using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetItemViewModel : AssetContainerContentItemContentViewModel
    {
        internal Asset Asset;

        public AssetItemViewModel(Asset asset)
        {
            Asset = asset;
        }

        public string Name => Asset.Info.Name;

        public Action<Asset>? OnOpen;
        public Action<Asset>? OnRemove;

        public override void Open()
        {
            OnOpen?.Invoke(Asset);
        }

        public override void Rename()
        {

        }
        public override void Remove()
        {
            OnRemove?.Invoke(Asset);
        }
        public override void Dispose()
        {

        }

        
    }
}
