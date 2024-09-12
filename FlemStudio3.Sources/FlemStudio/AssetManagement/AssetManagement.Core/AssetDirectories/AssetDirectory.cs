using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core.AssetDirectories
{
    public class AssetDirectory : IAssetContainer
    {
        
        public AssetDirectoryInfo Info { get; }
        IAssetContainerInfo IAssetContainer.Info => Info;
        public IAssetDirectoryDefinition Definition { get; }
        public AssetDirectory(AssetDirectoryInfo info, IAssetDirectoryDefinition definition)
        {
            Info = info;
            Definition = definition;
        }



        private IAssetContainer? _ParentContainer;
        public IAssetContainer? ParentContainer
        {
            get => _ParentContainer;
            set
            {
                if (_ParentContainer != null)
                {
                    _ParentContainer.RemoveChild(this);
                }
                if (value != null)
                {
                    value.AddChild(this);
                }
                _ParentContainer = value;
            }
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
