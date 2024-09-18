using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI.AssetDirectories
{
    public class MoveAssetDirectoryCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public MoveAssetDirectoryCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;


            Command = new Command("move", "Move an asset directory");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset directory will be created.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var currentDirectoryPathArgument = new Argument<string>(
                name: "current_directory_path",
                description: "The current path of the asset directory you want to move."
                );
            Command.AddArgument(currentDirectoryPathArgument);

            var destinationDirectoryPathArgument = new Argument<string>(
               name: "destination_directory_path",
               description: "The destination path of the asset directory you want to move."
               );
            Command.AddArgument(destinationDirectoryPathArgument);

            Command.SetHandler((rootDirectory, currentDirectoryPath, destinationDirectoryPath) =>
            {
                try
                {
                    AssetManager.MoveAssetDirectory(rootDirectory, currentDirectoryPath, destinationDirectoryPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, rootDirectoryOption, currentDirectoryPathArgument, destinationDirectoryPathArgument);


        }
    }
}
