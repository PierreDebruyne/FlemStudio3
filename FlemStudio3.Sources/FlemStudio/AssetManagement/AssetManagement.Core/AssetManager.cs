using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.AssetManagement.Core
{
    public class AssetManager
    {
        public string WorkingDirectory { get; }

        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public static string AssetDefinitionFileName { get; } = "Asset.yaml";
        public static string AssetDirectoryDefinitionFileName { get; } = "AssetDirectory.yaml";

        protected AssetTypeRegistry AssetTypeRegistry = new();


       

        public AssetRegistry AssetRegistry { get; }
        protected AssetRegistryUpdater AssetRegistryUpdater;

        public AssetManager(string assetFolderPath)
        {
            WorkingDirectory = FormatAssetPath(assetFolderPath);

            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            //AddAssetType(new AssetType(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Test", "0.0.1"));
            //AddAssetType(new AssetType(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Directory", "0.0.1"));

            AssetRegistry = new AssetRegistry(this);
            AssetRegistryUpdater = new AssetRegistryUpdater(AssetRegistry);

            RootAssetDirectoryInfo localAssetInfo = new RootAssetDirectoryInfo(this, "Local", "LocalAssets");

            //RootDirectoriesRegistry.Add(localAssetInfo.Name, localAssetInfo);

            AssetRegistry.RegisterRootAssetDirectory(localAssetInfo, true);
            

            //LocalAssetWatcher = new RootAssetFolderWatcher(LocalAssets);
        }



        public void Dispose()
        {
            AssetRegistryUpdater.Dispose();
        }

        public void Update(float deltaTime)
        {
            AssetRegistryUpdater.Update(deltaTime);
        }

        public void RegisterAssetType(AssetType assetType)
        {
            AssetTypeRegistry.RegisterAssetType(assetType);
            assetType.AssetManager = this;
        }

        public IEnumerable<AssetType> EnumerateAssetTypes()
        {
            return AssetTypeRegistry.EnumerateAssetTypes();
        }

        public bool TryGetAssetType(Guid guid, out AssetType? assetType)
        {
            return AssetTypeRegistry.TryGetAssetType(guid, out assetType);
        }

        

        
       
        

        public AssetDirectoryInfo CreateAssetDirectory(string rootDirectoryName, string? parentDirectoryPath, string name)
        {
            if (parentDirectoryPath != null)
            {
                parentDirectoryPath = FormatAssetPath(parentDirectoryPath);
            }

            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            if (parentDirectoryPath == null)
            {
                return CreateAssetDirectory(rootDirectory.Info, name);
            }

            AssetDirectoryInfo directoryInfo = rootDirectory.Info.GetAssetDirectoryInfo(parentDirectoryPath);
            return CreateAssetDirectory(directoryInfo, name);

            
        }

        public AssetDirectoryInfo CreateAssetDirectory(IAssetContainerInfo containerInfo, string name)
        {
            if (containerInfo.Exist == false)
            {
                throw new Exception("Parent directory does not exist: " + containerInfo.AssetPath);
            }


            AssetDirectoryInfo assetDirectoryInfo = containerInfo.GetAssetDirectoryInfo(name);
            

            if (assetDirectoryInfo.DefinitionFileExist)
            {
                throw new Exception("Asset directory already exist: " + assetDirectoryInfo.AssetPath);
            }


            AssetDirectoryDefinitionFile definitionFile = new()
            {
                Name = name,
                Guid = Guid.NewGuid(),
            };

            if (assetDirectoryInfo.FolderExist == false)
            {
                Directory.CreateDirectory(assetDirectoryInfo.FullPath);
            }
            using (TextWriter writer = File.CreateText(assetDirectoryInfo.DefinitionFileFullPath))
            {
                Serializer.Serialize(writer, definitionFile);
            }

            Console.WriteLine("Asset directory created: " + assetDirectoryInfo.AssetPath);
            return assetDirectoryInfo;
        }

        public AssetDirectoryInfo MoveAssetDirectory(string rootDirectoryName, string currentPath, string destinationPath)
        {
            currentPath = FormatAssetPath(currentPath);
            destinationPath = FormatAssetPath(destinationPath);

            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            AssetDirectoryInfo currentDirectoryInfo = rootDirectory.Info.GetAssetDirectoryInfo(currentPath);

            AssetDirectoryInfo destinationDirectoryInfo = rootDirectory.Info.GetAssetDirectoryInfo(destinationPath);

            return MoveAssetDirectory(currentDirectoryInfo, destinationDirectoryInfo);
        }

        public AssetDirectoryInfo MoveAssetDirectory(AssetDirectoryInfo currentDirectoryInfo, AssetDirectoryInfo destinationDirectoryInfo)
        {
            if (currentDirectoryInfo.Exist == false)
            {
                throw new Exception("Asset directory does not exist: " + currentDirectoryInfo.AssetPath);
            }
            if (destinationDirectoryInfo.Exist)
            {
                throw new Exception("Asset directory already exist: " + destinationDirectoryInfo.AssetPath);
            }

            AssetDirectoryDefinitionFile definitionFile;
            using (TextReader reader = File.OpenText(currentDirectoryInfo.FullPath + "/" + AssetDirectoryDefinitionFileName))
            {
                definitionFile = Deserializer.Deserialize<AssetDirectoryDefinitionFile>(reader);
            }

            Directory.Move(currentDirectoryInfo.FullPath, destinationDirectoryInfo.FullPath);
            if (definitionFile.Name == currentDirectoryInfo.Name && currentDirectoryInfo.Name != destinationDirectoryInfo.Name)
            {
                definitionFile.Name = destinationDirectoryInfo.Name;
                using (TextWriter writer = File.CreateText(destinationDirectoryInfo.FullPath + "/" + AssetDirectoryDefinitionFileName))
                {
                    Serializer.Serialize(writer, definitionFile);
                }
            }
            

            Console.WriteLine("Asset directory moved from '" + currentDirectoryInfo.AssetPath + "' to '" + destinationDirectoryInfo.AssetPath + "'.");
            return destinationDirectoryInfo;
        }

        public void RemoveAssetDirectory(string rootDirectoryName, string assetDirectoryPath, bool recursive = false)
        {
            assetDirectoryPath = FormatAssetPath(assetDirectoryPath);
            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            AssetDirectoryInfo directoryInfo = rootDirectory.Info.GetAssetDirectoryInfo(assetDirectoryPath);
            RemoveAssetDirectory(directoryInfo, recursive);
        }

        public void RemoveAssetDirectory(AssetDirectoryInfo directoryInfo, bool recursive = false)
        {
            if (directoryInfo.Exist == false)
            {
                throw new Exception("Asset directory does not exist: " + directoryInfo.AssetPath);
            }

            IEnumerable<AssetInfo> childAssets = directoryInfo.EnumerateAssets();
            IEnumerable<AssetDirectoryInfo> childDirectories = directoryInfo.EnumerateAssetDirectories();

            if (recursive)
            {
                foreach (AssetInfo childAsset in childAssets)
                {
                    RemoveAsset(childAsset);
                }

                foreach (AssetDirectoryInfo childDirectory in childDirectories)
                {
                    RemoveAssetDirectory(childDirectory, true);
                }
            }

            if (childAssets.Count() > 0 || childDirectories.Count() > 0)
            {
                throw new Exception("Asset directory is not empty: " + directoryInfo.AssetPath);
            }


            Directory.Delete(directoryInfo.FullPath, true);
            Console.WriteLine("Asset directory removed: " + directoryInfo.AssetPath);

        }


        

        /*
        public bool TryGetAsset(string assetPath, out Asset? asset)
        {
            string[] pathItems = assetPath.Split(":/");
            if (pathItems.Length == 0 || pathItems.Length > 2)
            {
                throw new Exception("Asset path must be in format: 'rootFolderName:/path/to/asset'");
            }

            return AssetsByPath.TryGetValue(assetPath, out asset);
        }
        */

        public AssetInfo CreateAsset(string assetTypeName, string rootDirectoryName, string? parentDirectoryPath, string name)
        {
            if (parentDirectoryPath != null)
            {
                parentDirectoryPath = FormatAssetPath(parentDirectoryPath);
            }

            AssetTypeRegistry.TryGetAssetType(assetTypeName, out AssetType? assetType);
            if (assetType == null)
            {
                throw new Exception("Asset type not found: " + assetTypeName);
            }

            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            if (parentDirectoryPath == null)
            {
                return CreateAsset(assetType, rootDirectory.Info, name);
            }

            AssetDirectoryInfo directoryInfo = rootDirectory.Info.GetAssetDirectoryInfo(parentDirectoryPath);
            return CreateAsset(assetType, directoryInfo, name);

        }

        public AssetInfo CreateAsset(AssetType assetType, IAssetContainerInfo containerInfo, string name)
        {
            if (containerInfo.Exist == false)
            {
                throw new Exception("Parent directory does not exist: " + containerInfo.AssetPath);
            }
            AssetInfo assetInfo = containerInfo.GetAssetInfo(name);
            
            if (assetInfo.DefinitionFileExist)
            {
                throw new Exception("Asset already exist: " + assetInfo.AssetPath);
            }


            AssetDefinitionFile definitionFile = new AssetDefinitionFile()
            {
                Guid = Guid.NewGuid(),
                Name = assetInfo.Name,
                AssetType = assetType.Guid,
                Version = assetType.Version,
            };
            if (assetInfo.FolderExist == false)
            {
                Directory.CreateDirectory(assetInfo.FullPath);
            }

            using (TextWriter writer = File.CreateText(assetInfo.DefinitionFileFullPath))
            {
                Serializer.Serialize(writer, definitionFile);
            }
            assetType.OnCreateAsset(assetInfo);
            Console.WriteLine("Asset of type '" + assetType.Name + "', version '" + assetType.Version + "' created: " + assetInfo.AssetPath);

            return assetInfo;

        }

        public AssetInfo MoveAsset(string rootDirectoryName, string currentPath, string destinationPath)
        {
            currentPath = FormatAssetPath(currentPath);
            destinationPath = FormatAssetPath(destinationPath);

            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            AssetInfo currentAssetInfo = rootDirectory.Info.GetAssetInfo(currentPath);

            AssetInfo destinationAssetInfo = rootDirectory.Info.GetAssetInfo(destinationPath);

            return MoveAsset(currentAssetInfo, destinationAssetInfo);
        }

        public AssetInfo MoveAsset(AssetInfo currentAssetInfo, AssetInfo destinationAssetInfo)
        {
            if (currentAssetInfo.Exist == false)
            {
                throw new Exception("Asset does not exist: " + currentAssetInfo.AssetPath);
            }
            if (destinationAssetInfo.Exist)
            {
                throw new Exception("Asset already exist: " + destinationAssetInfo.AssetPath);
            }

            AssetDefinitionFile definitionFile;
            using (TextReader reader = File.OpenText(currentAssetInfo.FullPath + "/" + AssetDefinitionFileName))
            {
                definitionFile = Deserializer.Deserialize<AssetDefinitionFile>(reader);
            }

            this.TryGetAssetType(definitionFile.AssetType, out AssetType? assetType);
            if (assetType == null)
            {
                throw new Exception("Unknown asset type. Something may broke because of an unmanaged move.");
            }

            Directory.Move(currentAssetInfo.FullPath, destinationAssetInfo.FullPath);
            if (definitionFile.Name == currentAssetInfo.Name && currentAssetInfo.Name != destinationAssetInfo.Name)
            {
                definitionFile.Name = destinationAssetInfo.Name;
                using (TextWriter writer = File.CreateText(destinationAssetInfo.FullPath + "/" + AssetDefinitionFileName))
                {
                    Serializer.Serialize(writer, definitionFile);
                }
            }
            assetType.OnMoveAsset(destinationAssetInfo);

            Console.WriteLine("Asset moved from '" + currentAssetInfo.AssetPath + "' to '" + destinationAssetInfo.AssetPath + "'.");
            return destinationAssetInfo;
        }

        public void RemoveAsset(string rootDirectoryName, string assetPath)
        {
            assetPath = FormatAssetPath(assetPath);
            if (AssetRegistry.TryGetRootAssetDirectory(rootDirectoryName, out RootAssetDirectory? rootDirectory) == false)
            {
                throw new Exception("Root asset directory '" + rootDirectoryName + "' not found.");
            }

            AssetInfo assetInfo = rootDirectory.Info.GetAssetInfo(assetPath);
            RemoveAsset(assetInfo);
        }

        public void RemoveAsset(AssetInfo assetInfo)
        {
            if (assetInfo.Exist == false)
            {
                throw new Exception("Asset does not exist: " + assetInfo.AssetPath);
            }

            AssetDefinitionFile definitionFile;
            using (TextReader reader = File.OpenText(assetInfo.FullPath + "/" + AssetDefinitionFileName))
            {
                definitionFile = Deserializer.Deserialize<AssetDefinitionFile>(reader);
            }

            this.TryGetAssetType(definitionFile.AssetType, out AssetType? assetType);
            if (assetType == null)
            {
                throw new Exception("Unknown asset type. Something may broke because of an unmanaged remove.");
            }

            Directory.Delete(assetInfo.FullPath, true);
            Console.WriteLine("Asset removed: " + assetInfo.AssetPath);

        }

        internal static string FormatAssetPath(string path)
        {
            path = path.Replace('\\', '/');
            path = path.Replace("//", "/");
            while (path.Last() == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        
    }

}
