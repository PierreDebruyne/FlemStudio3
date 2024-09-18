using FlemStudio.AssetManagement.Core.RootAssetDirectories;

namespace FlemStudio.AssetManagement.Core.Assets
{
    public class AssetInfo
    {
        protected RootAssetDirectoryInfo RootAssetDirectoryInfo;
        public string Path { get; }
        public string FullPath => RootAssetDirectoryInfo.FullPath + "/" + Path;
        public string AssetPath => RootAssetDirectoryInfo.AssetPath + "/" + Path;
        public string Name => Path.Split('/').Last();
        public string DefinitionFileFullPath => FullPath + "/" + AssetManager.AssetDefinitionFileName;

        public AssetInfo(RootAssetDirectoryInfo rootAssetDirectoryInfo, string path)
        {
            RootAssetDirectoryInfo = rootAssetDirectoryInfo;
            Path = path;
        }

        public bool FolderExist => Directory.Exists(FullPath);

        public bool DefinitionFileExist => File.Exists(DefinitionFileFullPath);

        public bool Exist => FolderExist && DefinitionFileExist;

        public IAssetContainerInfo GetParentInfo()
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
