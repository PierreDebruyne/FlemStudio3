namespace FlemStudio.AssetManagement.Core
{
    public interface IAssetInfo
    {
        public string Path { get; }
        public string FullPath { get; }
        public string AssetPath { get; }
        public string Name { get; }

        public string DefinitionFileFullPath { get; }

        public IAssetContainerInfo GetParentInfo();

    }
}
