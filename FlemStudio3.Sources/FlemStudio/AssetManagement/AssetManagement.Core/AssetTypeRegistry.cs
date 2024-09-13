using FlemStudio.ExtensionManagement.Core;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace FlemStudio.AssetManagement.Core
{
    public class AssetTypeDefinition
    {
        public string Name { get; }

        public Guid Guid { get; }

        public string Version { get; }

        public IAssetType AssetType { get; }

        public AssetTypeDefinition(string name, Guid guid, string version, IAssetType assetType)
        {
            Name = name;
            Guid = guid;
            Version = version;
            AssetType = assetType;
        }
    }

    public class AssetTypeRegistry
    {
        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public Lazy<IAssetType, IAssetTypeIdentity>[] LoadedAssetTypes { get; set; }


        protected Dictionary<Guid, AssetTypeDefinition> AssetTypesByGuid = new();
        protected Dictionary<string, AssetTypeDefinition> AssetTypesByName = new();

        public void LoadExtensions(ExtensionImporter extensionImporter)
        {




            extensionImporter.CompositionContainer.ComposeParts(this);

            foreach (var item in LoadedAssetTypes)
            {
                try
                {
                    RegisterAssetType(item.Metadata, item.Value);
                } catch (Exception e)
                {
                    Console.WriteLine("Impossible to load asset type:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public void TestExtensions()
        {
            foreach (var extension in LoadedAssetTypes)
            {
                Console.WriteLine("AssetType: '" + extension.Metadata.Guid + "', version: '" + extension.Metadata.Version + "'.");
                Console.WriteLine("Name: '" + extension.Value.Alias + "', description: '" + extension.Value.Description + "'.");
                extension.Value.Test();
            }
        }

        public void RegisterAssetType(IAssetTypeIdentity identity, IAssetType assetType)
        {
            Guid guid = Guid.Parse(identity.Guid);
            if (AssetTypesByGuid.ContainsKey(guid))
            {
                throw new Exception("Asset manager already have an asset type with guid: " + guid);
            }
            
            if (AssetTypesByName.ContainsKey(identity.Name))
            {
                throw new Exception("Asset manager already have an asset type with name: " + identity.Name);
            }
            AssetTypeDefinition definition = new AssetTypeDefinition(identity.Name, guid, identity.Version, assetType);
            AssetTypesByGuid.Add(guid, definition);
            AssetTypesByName.Add(identity.Name, definition);
        }

        public bool TryGetAssetType(Guid guid, out AssetTypeDefinition? assetType)
        {
            return AssetTypesByGuid.TryGetValue(guid, out assetType);
        }

        
        public bool TryGetAssetType(string name, out AssetTypeDefinition? assetType)
        {
            return AssetTypesByName.TryGetValue(name, out assetType);
        }
        

        public IEnumerable<AssetTypeDefinition> EnumerateAssetTypes()
        {
            foreach (AssetTypeDefinition assetType in AssetTypesByGuid.Values)
            {
                yield return assetType;
            }
        }
    }
}
