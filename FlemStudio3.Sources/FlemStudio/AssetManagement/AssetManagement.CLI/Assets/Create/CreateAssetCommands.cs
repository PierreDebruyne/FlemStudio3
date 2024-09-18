using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.ExtensionManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI.Assets.Create
{
    public class CreateAssetCommand
    {
        protected AssetManager AssetManager;
        protected AssetTypeDefinition AssetTypeDefinition;
        protected CreateAssetCommandTypeDefinition? CommandAssetTypeDefinition;

        public Command Command { get; }

        public CreateAssetCommand(AssetManager assetManager, AssetTypeDefinition assetTypeDefinition, CreateAssetCommandTypeDefinition? commandAssetTypeDefinition)
        {
            AssetManager = assetManager;
            AssetTypeDefinition = assetTypeDefinition;
            CommandAssetTypeDefinition = commandAssetTypeDefinition;

            Command = new Command(AssetTypeDefinition.Name, "Create an " + AssetTypeDefinition.AssetType.Alias + " asset.");
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

            if (CommandAssetTypeDefinition != null)
            {
                CommandAssetTypeDefinition.CreateAssetCommandType.ModifyCommand(Command);
            }

            Command.SetHandler(async (context) =>
            {
                string rootDirectory = context.ParseResult.GetValueForOption(rootDirectoryOption);
                string? directoryPath = context.ParseResult.GetValueForOption(directoryPathOption);
                string name = context.ParseResult.GetValueForArgument(nameArgument);

                try
                {

                    AssetManager.CreateAsset(AssetTypeDefinition, rootDirectory, directoryPath, name, (AssetInfo assetInfo) =>
                    {
                        if (CommandAssetTypeDefinition != null)
                        {
                            CommandAssetTypeDefinition.CreateAssetCommandType.OnCreateAsset(assetInfo, context);
                        }
                        else
                        {
                            AssetTypeDefinition.AssetType.OnCreateAssetDefault(assetInfo);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });
        }
    }

    public class CreateAssetCommands
    {
        protected AssetManager AssetManager;
        protected CreateAssetCommandRegistry CommandRegistry;
        public Command Command { get; }
        protected Dictionary<Guid, CreateAssetCommand> Commands = new();

        public CreateAssetCommands(AssetManager assetManager, ExtensionImporter extensionImporter)
        {
            AssetManager = assetManager;

            Command = new Command("create", "Create an asset.");

            CommandRegistry = new CreateAssetCommandRegistry(AssetManager);
            CommandRegistry.LoadExtensions(extensionImporter);
            CommandRegistry.TestExtensions();

            foreach (AssetTypeDefinition assetType in AssetManager.EnumerateAssetTypes())
            {
                CommandRegistry.TryGetCreateAssetCommandType(assetType.Guid, out CreateAssetCommandTypeDefinition? commandType);
                CreateAssetCommand createAssetCommand = new CreateAssetCommand(assetManager, assetType, commandType);
                Commands.Add(assetType.Guid, createAssetCommand);
                Command.AddCommand(createAssetCommand.Command);
            }


            /*
            

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

            */
        }
    }
}
