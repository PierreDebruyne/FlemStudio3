using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core
{
    public interface IAssetContainer
    {
        internal void AddChild(Asset asset);
        internal void RemoveChild(Asset asset);
        internal void AddChild(AssetDirectory directory);
        internal void RemoveChild(AssetDirectory directory);

        public IAssetContainerInfo Info { get; }

        public IAssetContainer? ParentContainer { get; }

        public Action<Asset>? OnAssetAdded { get; set; }
        public Action<Asset>? OnAssetRemoved { get; set; }
        public IEnumerable<Asset> EnumerateAssets();
        public Action<AssetDirectory>? OnAssetDirectoryAdded { get; set; }
        public Action<AssetDirectory>? OnAssetDirectoryRemoved { get; set; }
        public IEnumerable<AssetDirectory> EnumerateDirectories();
    }
}
