using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core
{
    public abstract class AssetType
    {
        public Guid Guid { get; }
        public string Name { get; }
        public string Version { get; }
        public abstract string Description { get; }

        internal AssetManager? AssetManager;

        public AssetType(Guid guid, string name, string version)
        {
            Guid = guid;
            Name = name;
            Version = version;
        }
        public abstract void OnCreateAsset(AssetInfo assetInfo);

        public abstract void OnMoveAsset(AssetInfo assetInfo);

        
    }
}
