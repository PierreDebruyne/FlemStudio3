using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core.RootAssetDirectories
{
    public class RootAssetDirectoryInfo : IAssetContainerInfo
    {
        internal AssetManager AssetManager { get; }
        public string Name { get; }
        public string Path { get; }
        public string FullPath => AssetManager.WorkingDirectory + "/" + Path;
        public string AssetPath => Name + ":";

        //public string DefinitionFileFullPath => FullPath + "/" + AssetManager.AssetDirectoryDefinitionFileName;

        public RootAssetDirectoryInfo(AssetManager assetManager, string name, string path)
        {
            AssetManager = assetManager;
            Name = name;
            Path = path;
        }

        public bool FolderExist => Directory.Exists(FullPath);

        //public bool DefinitionFileExist => File.Exists(DefinitionFileFullPath);
        public bool Exist => FolderExist;

        public AssetDirectoryInfo GetAssetDirectoryInfo(string path)
        {
            return new AssetDirectoryInfo(this, AssetManager.FormatAssetPath(path));
        }
        public AssetInfo GetAssetInfo(string path)
        {

            return new AssetInfo(this, AssetManager.FormatAssetPath(path));
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
            return null;
        }
    }
}
