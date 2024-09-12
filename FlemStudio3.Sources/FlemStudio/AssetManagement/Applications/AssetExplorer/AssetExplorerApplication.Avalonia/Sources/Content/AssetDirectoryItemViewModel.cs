using FlemStudio.AssetManagement.Core.AssetDirectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetDirectoryItemViewModel : AssetContainerContentItemViewModel
    {
        internal AssetDirectory AssetDirectory;

        public AssetDirectoryItemViewModel(AssetDirectory assetDirectory)
        {
            AssetDirectory = assetDirectory;
        }

        public string Name => AssetDirectory.Info.Name;

        public Action<AssetDirectory>? OnOpen;
        public void Open()
        {
            OnOpen?.Invoke(AssetDirectory);
        }

        public override void Dispose()
        {
            OnOpen = null;
        }
    }
}
