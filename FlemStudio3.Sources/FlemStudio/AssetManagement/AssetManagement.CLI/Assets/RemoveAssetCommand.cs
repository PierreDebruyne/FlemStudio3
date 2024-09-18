using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI.Assets
{
    public class RemoveAssetCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public RemoveAssetCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;

            Command = new Command("remove", "Remove an asset.");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset is located.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var assetPathOption = new Argument<string>(
                name: "asset_path",
                description: "The path of the asset you want to remove."
                );
            Command.AddArgument(assetPathOption);




            Command.SetHandler((rootDirectoryName, assetPath) =>
            {
                try
                {
                    AssetManager.RemoveAsset(rootDirectoryName, assetPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, rootDirectoryOption, assetPathOption);
        }
    }
}
