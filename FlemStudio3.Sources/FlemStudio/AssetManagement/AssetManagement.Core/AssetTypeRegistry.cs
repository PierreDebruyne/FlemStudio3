namespace FlemStudio.AssetManagement.Core
{
    public class AssetTypeRegistry
    {
        protected Dictionary<Guid, AssetType> AssetTypesByGuid = new();
        protected Dictionary<string, AssetType> AssetTypesByName = new();

        public void RegisterAssetType(AssetType assetType)
        {
            if (AssetTypesByGuid.ContainsKey(assetType.Guid))
            {
                throw new Exception("Asset manager already have an asset type with guid: " + assetType.Guid);
            }
            if (AssetTypesByName.ContainsKey(assetType.Name))
            {
                throw new Exception("Asset manager already have an asset type with name: " + assetType.Name);
            }
            AssetTypesByGuid.Add(assetType.Guid, assetType);
            AssetTypesByName.Add(assetType.Name, assetType);
        }

        public bool TryGetAssetType(Guid guid, out AssetType? assetType)
        {
            return AssetTypesByGuid.TryGetValue(guid, out assetType);
        }

        public bool TryGetAssetType(string name, out AssetType? assetType)
        {
            return AssetTypesByName.TryGetValue(name, out assetType);
        }

        public IEnumerable<AssetType> EnumerateAssetTypes()
        {
            foreach (AssetType assetType in AssetTypesByGuid.Values)
            {
                yield return assetType;
            }
        }
    }
}
