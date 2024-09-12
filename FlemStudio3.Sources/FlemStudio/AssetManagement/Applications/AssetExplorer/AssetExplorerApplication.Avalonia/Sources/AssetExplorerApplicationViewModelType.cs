using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Avalonia;
using FlemStudio.LayoutManagement.Avalonia.Applications;
using FlemStudio.LayoutManagement.Core.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetExplorerApplicationViewModelType : ApplicationViewModelType<AssetExplorerApplicationType, AssetExplorerApplicationViewModel, AssetExplorerApplicationUser>
    {
        public AssetManagerAvalonia AssetManagerAvalonia { get; }

        public AssetExplorerApplicationViewModelType(AssetExplorerApplicationType applicationType, AssetManagerAvalonia assetManagerAvalonia) : base(applicationType)
        {
            AssetManagerAvalonia = assetManagerAvalonia;
        }

        public override string Name => "Assets Explorer";

        public override AssetExplorerApplicationViewModel DoCreateApplicationViewModel(AssetExplorerApplicationUser user)
        {
            return new AssetExplorerApplicationViewModel(this, user);
        }
    }
}
