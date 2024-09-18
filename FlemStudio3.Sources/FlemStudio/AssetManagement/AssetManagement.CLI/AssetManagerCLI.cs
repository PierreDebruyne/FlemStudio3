using FlemStudio.AssetManagement.CLI.AssetDirectories;
using FlemStudio.AssetManagement.CLI.Assets;
using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;

namespace FlemStudio.AssetManagement.CLI
{
    public class AssetManagerCLI
    {
        protected AssetManager AssetManager;


        public AssetsCLI AssetsCLI { get; }
        public AssetDirectoriesCLI AssetDirectoryCLI { get; }

        public AssetManagerCLI(AssetManager assetManager, ExtensionImporter extensionImporter)
        {
            AssetManager = assetManager;

            AssetsCLI = new AssetsCLI(assetManager, extensionImporter);
            AssetDirectoryCLI = new AssetDirectoriesCLI(assetManager);
        }




    }



}
