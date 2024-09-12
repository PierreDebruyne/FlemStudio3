using FlemStudio.AssetManagement.CLI;

namespace FlemStudio.Project.CLI.ExtensionManagement
{
    public class AssetTypeCLIRegistry
    {
        protected Dictionary<Guid, AssetTypeCLI> AssetTypesByGuid = new();
        protected Dictionary<string, AssetTypeCLI> AssetTypesByName = new();

        public void RegisterAssetTypeCLI(AssetTypeCLI assetType)
        {
            if (AssetTypesByGuid.ContainsKey(assetType.Guid))
            {
                throw new Exception("Asset cli registry already have an asset type with guid: " + assetType.Guid);
            }
            if (AssetTypesByName.ContainsKey(assetType.Name))
            {
                throw new Exception("Asset cli registry already have an asset type with name: " + assetType.Name);
            }
            AssetTypesByGuid.Add(assetType.Guid, assetType);
            AssetTypesByName.Add(assetType.Name, assetType);
        }

        public bool TryGetAssetTypeCLI(Guid guid, out AssetTypeCLI? assetType)
        {
            return AssetTypesByGuid.TryGetValue(guid, out assetType);
        }

        public bool TryGetAssetTypeCLI(string name, out AssetTypeCLI? assetType)
        {
            return AssetTypesByName.TryGetValue(name, out assetType);
        }

        public IEnumerable<AssetTypeCLI> EnumerateAssetTypesCLI()
        {
            foreach (AssetTypeCLI assetType in AssetTypesByGuid.Values)
            {
                yield return assetType;
            }
        }
    }
}
