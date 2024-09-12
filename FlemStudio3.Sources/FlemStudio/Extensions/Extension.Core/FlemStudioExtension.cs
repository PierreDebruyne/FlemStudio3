using FlemStudio.AssetManagement;
using FlemStudio.AssetManagement.Core;

namespace FlemStudio.Extension.Core
{
    public abstract class FlemStudioExtension
    {
        internal AssetTypeRegistry AssetTypeRegistry;

        protected FlemStudioExtension()
        {
            AssetTypeRegistry = new();
        }

        public abstract void Test();
        public abstract void RegisterAssetTypes();
    }
}
