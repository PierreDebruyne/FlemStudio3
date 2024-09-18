using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;

namespace FlemStudio.AssetManagement.Core
{
    public interface IAssetContainerInfo
    {
        public IAssetContainerInfo? GetParentInfo();
        public AssetInfo GetAssetInfo(string path);
        public AssetDirectoryInfo GetAssetDirectoryInfo(string path);
        public IEnumerable<AssetInfo> EnumerateAssets();
        public IEnumerable<AssetDirectoryInfo> EnumerateAssetDirectories();

        public string Name { get; }
        public string Path { get; }
        public string FullPath { get; }
        public string AssetPath { get; }
        public bool Exist { get; }
    }
}
