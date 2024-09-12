

using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;

namespace FlemStudio.Project.Core.ExtensionManagement
{
    public abstract class ProjectExtension
    {
        public ExtensionInfo Infos { get; }
        internal AssetTypeRegistry AssetTypeRegistry;

        protected ProjectExtension(ExtensionInfo info)
        {
            Infos = info;
            AssetTypeRegistry = new();
        }

        public abstract void Test();

        protected void RegisterAssetType(AssetType assetType)
        {
            AssetTypeRegistry.RegisterAssetType(assetType);
        }
    }
}
