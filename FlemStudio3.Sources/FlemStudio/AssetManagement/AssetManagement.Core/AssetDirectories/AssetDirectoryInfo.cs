using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;

namespace FlemStudio.AssetManagement.Core.AssetDirectories
{
    public class AssetDirectoryInfo : IAssetContainerInfo
    {
        protected RootAssetDirectoryInfo RootAssetDirectoryInfo;
        public string Path { get; }
        public string FullPath => RootAssetDirectoryInfo.FullPath + "/" + Path;
        public string AssetPath => RootAssetDirectoryInfo.AssetPath + "/" + Path;
        public string Name => Path.Split('/').Last();

        public string DefinitionFileFullPath => FullPath + "/" + AssetManager.AssetDirectoryDefinitionFileName;

        public AssetDirectoryInfo(RootAssetDirectoryInfo rootAssetDirectoryInfo, string path)
        {
            RootAssetDirectoryInfo = rootAssetDirectoryInfo;
            Path = path;
        }
        public bool FolderExist => Directory.Exists(FullPath);

        public bool DefinitionFileExist => File.Exists(DefinitionFileFullPath);

        public bool Exist => FolderExist && DefinitionFileExist;

        public AssetInfo GetAssetInfo(string path)
        {
            return new AssetInfo(RootAssetDirectoryInfo, Path + "/" + path);
        }

        public AssetDirectoryInfo GetAssetDirectoryInfo(string path)
        {
            return new AssetDirectoryInfo(RootAssetDirectoryInfo, Path + "/" + path);
        }

        public IEnumerable<AssetInfo> EnumerateAssets()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(FullPath);

            foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
            {
                AssetInfo assetInfo = GetAssetInfo(childDirectoryInfo.Name);
                if (assetInfo.DefinitionFileExist)
                {
                    yield return assetInfo;
                }

            }
        }

        public IEnumerable<AssetDirectoryInfo> EnumerateAssetDirectories()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(FullPath);

            foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
            {
                AssetDirectoryInfo assetDirectoryInfo = GetAssetDirectoryInfo(childDirectoryInfo.Name);
                if (assetDirectoryInfo.DefinitionFileExist)
                {
                    yield return assetDirectoryInfo;
                }

            }
        }

        public IAssetContainerInfo? GetParentInfo()
        {
            
            string parentPath = string.Join("/", Path.Split("/").SkipLast(1));
            if (parentPath.Length == 0)
            {
                return RootAssetDirectoryInfo;
            }
            
            return RootAssetDirectoryInfo.GetAssetDirectoryInfo(parentPath);
        }
    }
}
