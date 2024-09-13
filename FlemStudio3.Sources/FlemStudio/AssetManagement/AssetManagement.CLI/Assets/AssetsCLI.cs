using FlemStudio.AssetManagement.CLI.Assets.Create;
using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.CLI.Assets
{
    public class AssetsCLI
    {
        protected AssetManager AssetManager;
        public Command Command { get; }
        protected CreateAssetCommands CreateAssetCommand { get; }
        protected MoveAssetCommand MoveAssetCommand { get; }
        protected RemoveAssetCommand RemoveAssetCommand { get; }

        public AssetsCLI(AssetManager assetManager, ExtensionImporter extensionImporter)
        {
            AssetManager = assetManager;
            Command = new Command("Asset", "Asset management");

            CreateAssetCommand = new CreateAssetCommands(assetManager, extensionImporter);
            Command.AddCommand(CreateAssetCommand.Command);

            MoveAssetCommand = new MoveAssetCommand(assetManager);
            Command.AddCommand(MoveAssetCommand.Command);

            RemoveAssetCommand = new RemoveAssetCommand(assetManager);
            Command.AddCommand(RemoveAssetCommand.Command);
        }
    }
}
