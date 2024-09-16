using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;

namespace FlemStudio.AssetManagement.Core
{
    public class AssetRegistry
    {
        public AssetManager AssetManager { get; }

        protected Dictionary<string, RootAssetDirectory> RootAssetDirectoriesByName = new();
        public Action<RootAssetDirectory>? OnRootAssetDirectoryAdded;
        
        protected Dictionary<string, IAssetContainer> AssetDirectoriesByPath = new();
        protected Dictionary<Guid, IAssetContainer> AssetDirectoriesByGuid = new();
        
        protected Dictionary<string, Asset> AssetsByPath = new();
        protected Dictionary<Guid, Asset> AssetsByGuid = new();

        public AssetRegistry(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }

        internal RootAssetDirectory RegisterRootAssetDirectory(RootAssetDirectoryInfo rootAssetDirectoryInfo, bool recursive = false)
        {
            RootAssetDirectory rootAssetDirectory = new RootAssetDirectory(rootAssetDirectoryInfo);
            if (RootAssetDirectoriesByName.ContainsKey(rootAssetDirectoryInfo.Name))
            {
                throw new Exception("Root asset directory path '" + rootAssetDirectory.Info.AssetPath + "' is already used.");
            }
            


            if (AssetDirectoriesByPath.ContainsKey(rootAssetDirectory.Info.AssetPath))
            {
                throw new Exception("Asset directory path '" + rootAssetDirectory.Info.AssetPath + "' is already used.");
            }
            AssetDirectoriesByPath.Add(rootAssetDirectory.Info.AssetPath, rootAssetDirectory);
            Console.WriteLine("Asset root directory registered: " + rootAssetDirectory.Info.AssetPath);

            if (recursive)
            {
                foreach (AssetInfo childAssetInfo in rootAssetDirectoryInfo.EnumerateAssets())
                {
                    try
                    {
                        Asset childAsset = RegisterAsset(childAssetInfo);
                        //childAsset.ParentDirectory = rootAssetDirectory;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                foreach (AssetDirectoryInfo childAssetDirectoryInfo in rootAssetDirectoryInfo.EnumerateAssetDirectories())
                {
                    try
                    {
                        AssetDirectory childDirectory = RegisterAssetDirectory(childAssetDirectoryInfo, recursive);
                        //childDirectory.ParentContainer = rootAssetDirectory;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            


            RootAssetDirectoriesByName.Add(rootAssetDirectory.Info.Name, rootAssetDirectory);
            OnRootAssetDirectoryAdded?.Invoke(rootAssetDirectory);
            return rootAssetDirectory;
        }



        internal AssetDirectory RegisterAssetDirectory(AssetDirectoryInfo assetDirectoryInfo, bool recursive = false)
        {
            if (AssetDirectoriesByPath.ContainsKey(assetDirectoryInfo.AssetPath))
            {
                throw new Exception("Asset directory path '" + assetDirectoryInfo.AssetPath + "' is already used.");
            }

            IAssetDirectoryDefinition definition;
            using (TextReader reader = File.OpenText(assetDirectoryInfo.DefinitionFileFullPath))
            {
                definition = AssetManager.Deserializer.Deserialize<AssetDirectoryDefinitionFile>(reader);
            }
            AssetDirectory assetDirectory = new AssetDirectory(assetDirectoryInfo, definition);
           
            if (AssetDirectoriesByGuid.ContainsKey(assetDirectory.Definition.Guid))
            {
                throw new Exception("Asset directory guid '" + assetDirectory.Definition.Guid + "' is already used.");
            }


            IAssetContainerInfo? parentInfo = assetDirectoryInfo.GetParentInfo();

            if (parentInfo != null && this.AssetDirectoriesByPath.TryGetValue(parentInfo.AssetPath, out IAssetContainer? parentContainer)) {
                assetDirectory.ParentContainer = parentContainer;
            }

            AssetDirectoriesByPath.Add(assetDirectory.Info.AssetPath, assetDirectory);
            AssetDirectoriesByGuid.Add(assetDirectory.Definition.Guid, assetDirectory);
            Console.WriteLine("Asset directory registered: " + assetDirectory.Info.AssetPath);

            if (recursive)
            {
                foreach (AssetInfo childAssetInfo in assetDirectoryInfo.EnumerateAssets())
                {
                    try
                    {
                        Asset childAsset = RegisterAsset(childAssetInfo);
                        //childAsset.ParentDirectory = assetDirectory;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                foreach (AssetDirectoryInfo childAssetDirectoryInfo in assetDirectoryInfo.EnumerateAssetDirectories())
                {
                    try
                    {
                        AssetDirectory childDirectory = RegisterAssetDirectory(childAssetDirectoryInfo, recursive);
                        //childDirectory.ParentContainer = assetDirectory;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            
            
            return assetDirectory;

        }

        internal Asset RegisterAsset(IAssetInfo assetInfo)
        {
            if (AssetsByPath.ContainsKey(assetInfo.AssetPath))
            {
                throw new Exception("Asset path '" + assetInfo.AssetPath + "' is already used.");
            }
            IAssetDefinition definition;
            using (TextReader reader = File.OpenText(assetInfo.DefinitionFileFullPath))
            {
                definition = AssetManager.Deserializer.Deserialize<AssetDefinitionFile>(reader);
            }
            Asset asset = new Asset(assetInfo, definition);
            
            if (AssetsByGuid.ContainsKey(asset.Definition.Guid))
            {
                throw new Exception("Asset guid '" + asset.Definition.Guid + "' is already used.");
            }

            IAssetContainerInfo? parentInfo = assetInfo.GetParentInfo();
            if (parentInfo != null && this.AssetDirectoriesByPath.TryGetValue(parentInfo.AssetPath, out IAssetContainer? parentContainer))
            {
                asset.ParentDirectory = parentContainer;
            }

            AssetsByPath.Add(asset.Info.AssetPath, asset);
            AssetsByGuid.Add(asset.Definition.Guid, asset);
            Console.WriteLine("Asset registered: " + asset.Info.AssetPath);
            return asset;
        }


        public bool TryGetAssetContainer(string assetPath, out IAssetContainer? assetContainer)
        {
            assetPath = AssetManager.FormatAssetPath(assetPath);
            return AssetDirectoriesByPath.TryGetValue(assetPath, out assetContainer);
        }

        public bool TryGetRootAssetDirectory(string rootDirectoryName, out RootAssetDirectory? rootAssetDirectory)
        {
            return RootAssetDirectoriesByName.TryGetValue(rootDirectoryName, out rootAssetDirectory);
        }

        public bool TryGetAssetDirectory(string assetPath, out AssetDirectory? assetDirectory)
        {
            assetPath = AssetManager.FormatAssetPath(assetPath);
            if (AssetDirectoriesByPath.TryGetValue(assetPath, out IAssetContainer? assetContainer))
            {
                if (assetContainer is AssetDirectory)
                {
                    assetDirectory = (AssetDirectory)assetContainer;
                    return true;
                }
            }
            assetDirectory = null;
            return false;
        }

        public bool TryGetAsset(string assetPath, out Asset? asset)
        {
            assetPath = AssetManager.FormatAssetPath(assetPath);
            return AssetsByPath.TryGetValue(assetPath, out asset);
        }

        public IEnumerable<RootAssetDirectory> EnumerateRootAssetDirectory()
        {
            foreach (RootAssetDirectory rootAssetDirectory in RootAssetDirectoriesByName.Values)
            {
                yield return rootAssetDirectory;
            }
        }

        /*
        public void UnregisterAsset(IAssetInfo info)
        {
            if (AssetsByPath.TryGetValue(info.AssetPath, out Asset? asset) == false)
            {
                throw new Exception("This asset is not registered: " + info.AssetPath);
            }
            UnregisterAsset(asset);

        }
        */

        public void UnregisterAsset(Asset asset)
        {
            asset.ParentDirectory?.RemoveChild(asset);
            AssetsByPath.Remove(asset.Info.AssetPath);
            AssetsByGuid.Remove(asset.Definition.Guid);
        }

        public void UnregisterAssetDirectory(AssetDirectory directory)
        {
            AssetDirectory? childDirectory = directory.FirstDirectory();
            while (childDirectory != null)
            {
                UnregisterAssetDirectory(childDirectory);
                childDirectory = directory.FirstDirectory();
            }
            Asset? childAsset = directory.FirstAsset();
            while (childAsset != null)
            {
                UnregisterAsset(childAsset);
                childAsset = directory.FirstAsset();
            }
            

            directory.ParentContainer?.RemoveChild(directory);
            AssetDirectoriesByPath.Remove(directory.Info.AssetPath);
            AssetDirectoriesByGuid.Remove(directory.Definition.Guid);


        }

        
    }
}
