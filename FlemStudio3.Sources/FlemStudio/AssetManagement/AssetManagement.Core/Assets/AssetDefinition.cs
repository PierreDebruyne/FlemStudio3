namespace FlemStudio.AssetManagement.Core.Assets
{
    public interface IAssetDefinition
    {
        public Guid Guid { get; }
        public string Name { get; }
        public Guid AssetType { get; }
        public string Version { get; }
    }
    public class AssetDefinitionFile : IAssetDefinition
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Guid AssetType { get; set; }
        public string Version { get; set; }
    }
}
