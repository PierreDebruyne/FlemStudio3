using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace FlemStudio.AssetManagement.Avalonia
{
    public class CreateAssetDialogTypeDefinition
    {
        public string Name { get; }

        public Guid Guid { get; }

        public string Version { get; }

        public ICreateAssetDialogType CreateAssetDialogType { get; }



        public CreateAssetDialogTypeDefinition(string name, Guid guid, string version, ICreateAssetDialogType createAssetDialogType)
        {
            Name = name;
            Guid = guid;
            Version = version;
            CreateAssetDialogType = createAssetDialogType;


        }


    }
    public class CreateAssetDialogRegistry
    {
        protected AssetManager AssetManager;

        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public Lazy<ICreateAssetDialogType, IAssetTypeIdentity>[] LoadedDialogTypes { get; set; }

        protected Dictionary<Guid, CreateAssetDialogTypeDefinition> DialogTypesByGuid = new();
        protected Dictionary<string, CreateAssetDialogTypeDefinition> DialogTypesByName = new();

        public CreateAssetDialogRegistry(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }

        public void LoadExtensions(ExtensionImporter extensionImporter)
        {
            extensionImporter.CompositionContainer.ComposeParts(this);


            foreach (var item in LoadedDialogTypes)
            {
                try
                {
                    RegisterCreateAssetDialogType(item.Metadata, item.Value);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Impossible to load CreateAssetDialogType: '" + item.Metadata.Name + "'.");
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                }
            }

        }

        public void TestExtensions()
        {
            foreach (var extension in LoadedDialogTypes)
            {
                Debug.WriteLine("CreateAssetDialogType: '" + extension.Metadata.Guid + "', version: '" + extension.Metadata.Version + "'.");
                //Debug.WriteLine("Name: '" + extension.Value.Alias + "', description: '" + extension.Value.Description + "'.");
                extension.Value.Test();
            }
        }


        public void RegisterCreateAssetDialogType(IAssetTypeIdentity identity, ICreateAssetDialogType dialogType)
        {


            Guid guid = Guid.Parse(identity.Guid);
            if (DialogTypesByGuid.ContainsKey(guid))
            {
                throw new Exception("Create asset dialog registry already have a command type with guid: " + guid);
            }

            if (DialogTypesByName.ContainsKey(identity.Name))
            {
                throw new Exception("Create asset dialog registry already have a command type name: " + identity.Name);
            }

            /*
            if (AssetManager.TryGetAssetType(guid, out AssetTypeDefinition? assetType) == false) {
                throw new Exception("Cannot find asset type.");
            }
            */

            CreateAssetDialogTypeDefinition definition = new CreateAssetDialogTypeDefinition(identity.Name, guid, identity.Version, dialogType);
            DialogTypesByGuid.Add(guid, definition);
            DialogTypesByName.Add(identity.Name, definition);


        }

        public bool TryGetCreateAssetDialogType(Guid guid, out CreateAssetDialogTypeDefinition? commandType)
        {
            return DialogTypesByGuid.TryGetValue(guid, out commandType);
        }


        public bool TryGetCreateAssetDialogType(string name, out CreateAssetDialogTypeDefinition? commandType)
        {
            return DialogTypesByName.TryGetValue(name, out commandType);
        }


        public IEnumerable<CreateAssetDialogTypeDefinition> EnumerateCreateAssetCommandTypes()
        {
            foreach (CreateAssetDialogTypeDefinition commandType in DialogTypesByGuid.Values)
            {
                yield return commandType;
            }
        }
    }
}
