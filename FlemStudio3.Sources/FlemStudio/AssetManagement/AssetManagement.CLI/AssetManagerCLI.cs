using FlemStudio.AssetManagement.CLI.AssetDirectories;
using FlemStudio.AssetManagement.CLI.Assets;
using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI
{
    public class AssetManagerCLI
    {
        protected AssetManager AssetManager;
        

        public AssetsCLI AssetsCLI { get; }
        public AssetDirectoriesCLI AssetDirectoryCLI { get; }

        public AssetManagerCLI(AssetManager assetManager)
        {
            AssetManager = assetManager;

            AssetsCLI = new AssetsCLI(assetManager);
            AssetDirectoryCLI = new AssetDirectoriesCLI(assetManager);
        }

       


    }
}
