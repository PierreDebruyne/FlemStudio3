using FlemStudio.AssetManagement.Core;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.CLI.AssetDirectories
{
    public class AssetDirectoriesCLI
    {
        protected AssetManager AssetManager;
        public Command Command { get; }
        
        protected CreateAssetDirectoryCommand CreateAssetDirectoryCommand;
        protected MoveAssetDirectoryCommand MoveAssetDirectoryCommand;
        protected RemoveAssetDirectoryCommand RemoveAssetDirectoryCommand;

        public AssetDirectoriesCLI(AssetManager assetManager)
        {
            AssetManager = assetManager;
            Command = new Command("AssetDirectory", "Asset directories management");


            CreateAssetDirectoryCommand = new CreateAssetDirectoryCommand(assetManager);
            Command.AddCommand(CreateAssetDirectoryCommand.Command);

            MoveAssetDirectoryCommand = new MoveAssetDirectoryCommand(assetManager);
            Command.AddCommand(MoveAssetDirectoryCommand.Command);

            RemoveAssetDirectoryCommand = new RemoveAssetDirectoryCommand(assetManager);
            Command.AddCommand(RemoveAssetDirectoryCommand.Command);
        }
    }
}
