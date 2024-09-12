using FlemStudio.AssetManagement.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetItemViewModel : AssetContainerContentItemViewModel
    {
        internal Asset Asset;

        public AssetItemViewModel(Asset asset)
        {
            Asset = asset;
        }

        public string Name => Asset.Info.Name;

        public override void Dispose()
        {
            
        }
    }
}
