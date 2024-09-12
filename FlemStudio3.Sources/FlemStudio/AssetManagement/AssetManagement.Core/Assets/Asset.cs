namespace FlemStudio.AssetManagement.Core.Assets
{
    public class AssetConfig
    {
        public Guid Guid { get; }
        public string Name { get; }
        public AssetType AssetType { get; }
        public string Version { get; }

        public AssetConfig(Guid guid, string name, AssetType assetType, string version)
        {
            Guid = guid;
            Name = name;
            AssetType = assetType;
            Version = version;
        }
    }



    public class Asset
    {
        

        public IAssetInfo Info { get; }
        public IAssetDefinition Definition { get; }


        public Asset(IAssetInfo info, IAssetDefinition definition)
        {
            Info = info;
            Definition = definition;
            

        }
        private IAssetContainer? _ParentDirectory;
        public IAssetContainer? ParentDirectory
        {
            get => _ParentDirectory;
            set
            {
                if (_ParentDirectory != null)
                {
                    _ParentDirectory.RemoveChild(this);
                }
                if (value != null)
                {
                    value.AddChild(this);
                }
                _ParentDirectory = value;
            }
        }
    }


}
