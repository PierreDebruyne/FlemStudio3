using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Avalonia;
using FlemStudio.LayoutManagement.Avalonia.Applications;

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
