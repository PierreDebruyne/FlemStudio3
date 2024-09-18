using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI.Assets
{
    public class MoveAssetCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public MoveAssetCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;

            Command = new Command("move", "Move an asset.");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset is located.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var currentAssetPathOption = new Argument<string>(
                name: "current_asset_path",
                description: "The current path of the asset you want to move."
                );
            Command.AddArgument(currentAssetPathOption);

            var destinationAssetPathOption = new Argument<string>(
               name: "destination_asset_path",
               description: "The destination path of the asset you want to move."
               );
            Command.AddArgument(destinationAssetPathOption);


            Command.SetHandler((rootDirectoryName, currentAssetPath, destinationAssetPath) =>
            {
                try
                {
                    AssetManager.MoveAsset(rootDirectoryName, currentAssetPath, destinationAssetPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, rootDirectoryOption, currentAssetPathOption, destinationAssetPathOption);
        }
    }
}
