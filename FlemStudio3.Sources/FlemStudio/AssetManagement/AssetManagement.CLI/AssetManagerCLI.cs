using FlemStudio.AssetManagement.CLI.AssetDirectories;
using FlemStudio.AssetManagement.CLI.Assets;
using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI
{
    public class AssetManagerCLI
    {
        protected AssetManager AssetManager;
        public AssetTypeCLIRegistry AssetTypeRegistry { get; } = new();

        public AssetsCLI AssetsCLI { get; }
        public AssetDirectoriesCLI AssetDirectoryCLI { get; }

        public AssetManagerCLI(AssetManager assetManager)
        {
            AssetManager = assetManager;

            AssetsCLI = new AssetsCLI(assetManager);
            AssetDirectoryCLI = new AssetDirectoriesCLI(assetManager);
        }

        public void RegisterAssetTypeCLI(AssetTypeCLI assetTypeCLI)
        {
            AssetManager.TryGetAssetType(assetTypeCLI.Guid, out AssetType? assetType);
            if (assetType == null)
            {
                throw new Exception("Asset type not found for asset type cli: " + assetTypeCLI.Name);
            }
            AssetTypeRegistry.RegisterAssetType(assetTypeCLI);
            

            assetTypeCLI.AssetTypeBase = assetType;
        }


    }
}
