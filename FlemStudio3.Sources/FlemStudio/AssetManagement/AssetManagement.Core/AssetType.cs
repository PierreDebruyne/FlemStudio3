using FlemStudio.AssetManagement.Core.Assets;
using System.ComponentModel.Composition;
using System.Xml.Linq;

namespace FlemStudio.AssetManagement.Core
{

    public interface IAssetType
    {
        public string Alias { get; }
        public string Description { get; }

        public void Test();
        public void OnCreateAsset(AssetInfo assetInfo);
        public void OnMoveAsset(AssetInfo assetInfo);
        public void OnDeleteAsset(AssetInfo assetInfo);
    }


    public interface IAssetTypeIdentity
    {
        public string Name { get; }
        public string Guid { get; }
        public string Version { get; }
    }



    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AssetTypeAttribute : ExportAttribute, IAssetTypeIdentity
    {
        public AssetTypeAttribute(string name, string guid, string version) : base(typeof(IAssetType)) 
        {
            Name = name;
            Guid = guid;
            Version = version;
        }

        public string Name { get; }
        public string Guid { get; }
        public string Version { get; }

       
    }


    

}
