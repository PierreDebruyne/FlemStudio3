using FlemStudio.AssetManagement.Core;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.CLI.AssetDirectories
{
    public class RemoveAssetDirectoryCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public RemoveAssetDirectoryCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;

            Command = new Command("remove", "Remove an asset directory.");

            var rootDirectoryOption = new Option<string>(
                name: "--root",
                description: "The root asset directory where the asset directory is located.",
                getDefaultValue: () => "Local"
                );
            Command.AddOption(rootDirectoryOption);

            var directoryPathArgument = new Argument<string>(
                name: "directory_path",
                description: "The path of the asset directory you want to remove."
                );
            Command.AddArgument(directoryPathArgument);

            var recursiveOption = new Option<bool>(
                name: "--r",
                description: "Remove all child assets and directories."
                );
            Command.AddOption(recursiveOption);

            Command.SetHandler((rootDirectoryName, directoryPath, recursive) =>
            {
                try
                {
                    Console.WriteLine("Remove directory " + rootDirectoryName + ":/" + directoryPath + " recursive: " + recursive);
                    AssetManager.RemoveAssetDirectory(rootDirectoryName, directoryPath, recursive);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, rootDirectoryOption, directoryPathArgument, recursiveOption);
        }
    }
}
