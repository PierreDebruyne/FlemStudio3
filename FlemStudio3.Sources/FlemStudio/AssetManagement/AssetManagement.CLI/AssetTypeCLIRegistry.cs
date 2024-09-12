namespace FlemStudio.AssetManagement.CLI
{
    public class AssetTypeCLIRegistry
    {
        protected Dictionary<Guid, AssetTypeCLI> AssetTypesByGuid = new();
        protected Dictionary<string, AssetTypeCLI> AssetTypesByName = new();

        public void RegisterAssetType(AssetTypeCLI assetType)
        {
            if (AssetTypesByGuid.ContainsKey(assetType.Guid))
            {
                throw new Exception("Asset manager CLI already have an asset type with guid: " + assetType.Guid);
            }
            if (AssetTypesByName.ContainsKey(assetType.Name))
            {
                throw new Exception("Asset manager CLI already have an asset type with name: " + assetType.Name);
            }
            AssetTypesByGuid.Add(assetType.Guid, assetType);
            AssetTypesByName.Add(assetType.Name, assetType);
        }

        public bool TryGetAssetType(Guid guid, out AssetTypeCLI? assetType)
        {
            return AssetTypesByGuid.TryGetValue(guid, out assetType);
        }

        public bool TryGetAssetType(string name, out AssetTypeCLI? assetType)
        {
            return AssetTypesByName.TryGetValue(name, out assetType);
        }

        public IEnumerable<AssetTypeCLI> EnumerateAssetTypes()
        {
            foreach (AssetTypeCLI assetType in AssetTypesByGuid.Values)
            {
                yield return assetType;
            }
        }
    }
}
