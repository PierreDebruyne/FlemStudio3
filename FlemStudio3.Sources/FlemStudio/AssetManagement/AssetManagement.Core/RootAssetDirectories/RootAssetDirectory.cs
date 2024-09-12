using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core.RootAssetDirectories
{


    public class RootAssetDirectory : IAssetContainer
    {
        public RootAssetDirectoryInfo Info { get; }

        IAssetContainerInfo IAssetContainer.Info => Info;

        public IAssetContainer? ParentContainer => null;

        public RootAssetDirectory(RootAssetDirectoryInfo info)
        {
            Info = info;
        }

        public Action<Asset>? OnAssetAdded { get; set; }
        public Action<Asset>? OnAssetRemoved { get; set; }

        protected List<Asset> ChildAssets = new();

        public Action<AssetDirectory>? OnAssetDirectoryAdded { get; set; }
        public Action<AssetDirectory>? OnAssetDirectoryRemoved { get; set; }
        protected List<AssetDirectory> ChildDirectories = new();

        void IAssetContainer.AddChild(Asset asset)
        {
            ChildAssets.Add(asset);
            OnAssetAdded?.Invoke(asset);
        }

        void IAssetContainer.RemoveChild(Asset asset)
        {
            ChildAssets.Remove(asset);
            OnAssetRemoved?.Invoke(asset);
        }

        void IAssetContainer.AddChild(AssetDirectory directory)
        {
            ChildDirectories.Add(directory);
            OnAssetDirectoryAdded?.Invoke(directory);
        }

        void IAssetContainer.RemoveChild(AssetDirectory directory)
        {
            ChildDirectories.Remove(directory);
            OnAssetDirectoryRemoved?.Invoke(directory);
        }

        public IEnumerable<Asset> EnumerateAssets()
        {
            return ChildAssets;
        }

        public IEnumerable<AssetDirectory> EnumerateDirectories()
        {
            return ChildDirectories;
        }
    }
}
