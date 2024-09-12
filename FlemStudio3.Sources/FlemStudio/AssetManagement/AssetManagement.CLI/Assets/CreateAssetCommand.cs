using FlemStudio.AssetManagement.Core;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.CLI.Assets
{
    public class CreateAssetCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public CreateAssetCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;

            Command = new Command("create", "Create an asset.");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset will be created.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var directoryPathOption = new Option<string?>(
                name: "--path",
                description: "The directory path where the asset will be created.",
                getDefaultValue: () => null
                );
            Command.AddOption(directoryPathOption);

            var nameArgument = new Argument<string>(
               name: "name",
               description: "The name of the asset you want to create."
               );
            Command.AddArgument(nameArgument);

            var assetTypeOption = new Option<string>(
                name: "--type",
                description: "The type of the asset you want to create."
                )
            {
                IsRequired = true,
                AllowMultipleArgumentsPerToken = false,
            };
            Command.AddOption(assetTypeOption);

            Command.SetHandler((assetTypeName, rootDirectory, directoryPath, name) =>
            {
                try
                {
                    AssetManager.CreateAsset(assetTypeName, rootDirectory, directoryPath, name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, assetTypeOption, rootDirectoryOption, directoryPathOption, nameArgument);
        }
    }
}
