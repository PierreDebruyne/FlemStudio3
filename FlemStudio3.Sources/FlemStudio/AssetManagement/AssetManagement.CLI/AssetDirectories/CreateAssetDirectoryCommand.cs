using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI.AssetDirectories
{
    public class CreateAssetDirectoryCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public CreateAssetDirectoryCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;


            Command = new Command("create", "Create an asset directory");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset directory will be created.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var directoryPathOption = new Option<string?>(
                name: "--path",
                description: "The directory path where the asset directory will be created.",
                getDefaultValue: () => null
                );
            Command.AddOption(directoryPathOption);

            var nameArgument = new Argument<string>(
               name: "name",
               description: "The name of the asset directory you want to create.'"
               );
            Command.AddArgument(nameArgument);

            Command.SetHandler((rootDirectory, directoryPath, name) =>
            {
                try
                {
                    AssetManager.CreateAssetDirectory(rootDirectory, directoryPath, name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, rootDirectoryOption, directoryPathOption, nameArgument);


        }
    }
}
