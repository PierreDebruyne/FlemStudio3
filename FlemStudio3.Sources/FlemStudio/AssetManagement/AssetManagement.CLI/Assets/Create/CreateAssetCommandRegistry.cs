using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;
using System.ComponentModel.Composition;

namespace FlemStudio.AssetManagement.CLI.Assets.Create
{
    public class CreateAssetCommandTypeDefinition
    {
        public string Name { get; }

        public Guid Guid { get; }

        public string Version { get; }

        public ICreateAssetCommandType CreateAssetCommandType { get; }



        public CreateAssetCommandTypeDefinition(string name, Guid guid, string version, ICreateAssetCommandType createAssetCommandType)
        {
            Name = name;
            Guid = guid;
            Version = version;
            CreateAssetCommandType = createAssetCommandType;


        }


    }
    public class CreateAssetCommandRegistry
    {
        protected AssetManager AssetManager;

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public Lazy<ICreateAssetCommandType, IAssetTypeIdentity>[] LoadedCommandTypes { get; set; }

        protected Dictionary<Guid, CreateAssetCommandTypeDefinition> CommandTypesByGuid = new();
        protected Dictionary<string, CreateAssetCommandTypeDefinition> CommandTypesByName = new();

        public CreateAssetCommandRegistry(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }

        public void LoadExtensions(ExtensionImporter extensionImporter)
        {
            extensionImporter.CompositionContainer.ComposeParts(this);


            foreach (var item in LoadedCommandTypes)
            {
                try
                {
                    RegisterCreateAssetCommandType(item.Metadata, item.Value);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible to load CreateAssetCommandType: '" + item.Metadata.Name + "'.");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

        }

        public void TestExtensions()
        {
            foreach (var extension in LoadedCommandTypes)
            {
                Console.WriteLine("CreateAssetCommandType: '" + extension.Metadata.Guid + "', version: '" + extension.Metadata.Version + "'.");
                //Console.WriteLine("Name: '" + extension.Value.Alias + "', description: '" + extension.Value.Description + "'.");
                extension.Value.Test();
            }
        }


        public void RegisterCreateAssetCommandType(IAssetTypeIdentity identity, ICreateAssetCommandType commandType)
        {


            Guid guid = Guid.Parse(identity.Guid);
            if (CommandTypesByGuid.ContainsKey(guid))
            {
                throw new Exception("Create asset command registry already have a command type with guid: " + guid);
            }

            if (CommandTypesByName.ContainsKey(identity.Name))
            {
                throw new Exception("Create asset command registry already have a command type name: " + identity.Name);
            }

            /*
            if (AssetManager.TryGetAssetType(guid, out AssetTypeDefinition? assetType) == false) {
                throw new Exception("Cannot find asset type.");
            }
            */

            CreateAssetCommandTypeDefinition definition = new CreateAssetCommandTypeDefinition(identity.Name, guid, identity.Version, commandType);
            CommandTypesByGuid.Add(guid, definition);
            CommandTypesByName.Add(identity.Name, definition);


        }

        public bool TryGetCreateAssetCommandType(Guid guid, out CreateAssetCommandTypeDefinition? commandType)
        {
            return CommandTypesByGuid.TryGetValue(guid, out commandType);
        }


        public bool TryGetCreateAssetCommandType(string name, out CreateAssetCommandTypeDefinition? commandType)
        {
            return CommandTypesByName.TryGetValue(name, out commandType);
        }


        public IEnumerable<CreateAssetCommandTypeDefinition> EnumerateCreateAssetCommandTypes()
        {
            foreach (CreateAssetCommandTypeDefinition commandType in CommandTypesByGuid.Values)
            {
                yield return commandType;
            }
        }
    }
}
