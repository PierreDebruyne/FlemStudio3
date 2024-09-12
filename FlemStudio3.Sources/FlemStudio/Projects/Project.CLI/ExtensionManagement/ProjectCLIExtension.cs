using FlemStudio.AssetManagement.CLI;
using FlemStudio.ExtensionManagement.Core;

namespace FlemStudio.Project.CLI.ExtensionManagement
{
    public abstract class ProjectCLIExtension
    {
        public ExtensionInfo Infos { get; }
        internal AssetTypeCLIRegistry AssetTypeRegistry;

        protected ProjectCLIExtension(ExtensionInfo info)
        {
            Infos = info;
            AssetTypeRegistry = new();
        }
        public abstract void Test();

        protected void RegisterAssetTypeCLI(AssetTypeCLI assetType)
        {
            AssetTypeRegistry.RegisterAssetTypeCLI(assetType);
        }
    }
}
