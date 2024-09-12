namespace FlemStudio.AssetManagement.Core.AssetDirectories
{
    public interface IAssetDirectoryDefinition
    {
        public Guid Guid { get; }
        public string Name { get; }
    }
    public class AssetDirectoryDefinitionFile : IAssetDirectoryDefinition
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }


    }
}
