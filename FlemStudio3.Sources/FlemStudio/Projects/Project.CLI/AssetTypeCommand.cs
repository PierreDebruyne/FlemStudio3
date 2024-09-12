using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.Project.CLI
{
    public class AssetTypeCommand
    {
        protected AssetManager AssetManager;
        public Command Command { get; }

        public AssetTypeCommand(AssetManager assetManager)
        {
            AssetManager = assetManager;

            Command = new Command("AssetTypes", "Available asset types management.");


            Command listCommand = new Command("list", "List all available asset types.");
            listCommand.SetHandler(() =>
            {
                foreach (AssetTypeDefinition assetType in AssetManager.EnumerateAssetTypes())
                {
                    Console.WriteLine(assetType.Name + " => version: '" + assetType.Version + "', guid: '" + assetType.Guid + "'");
                }
            });
            Command.AddCommand(listCommand);
        }
    }
}
