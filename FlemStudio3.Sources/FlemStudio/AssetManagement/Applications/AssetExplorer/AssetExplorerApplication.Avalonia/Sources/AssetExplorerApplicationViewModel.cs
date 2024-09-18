using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using FlemStudio.LayoutManagement.Avalonia.Applications;
using ReactiveUI;
using System.Diagnostics;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetExplorerApplicationViewModel : ApplicationViewModel<AssetExplorerApplicationViewModelType, AssetExplorerApplicationUser>
    {
        public AssetBreadCrumbViewModel AssetBreadCrumbViewModel { get; }
        public AssetContainerContentViewModel AssetContainerContentViewModel { get; }

        public AssetExplorerApplicationViewModel(AssetExplorerApplicationViewModelType applicationViewModelType, AssetExplorerApplicationUser applicationUser) : base(applicationViewModelType, applicationUser)
        {
            ApplicationUser.OnCurrentAssetPathUpdated += OnCurrentAssetPathUpdated;
            AssetBreadCrumbViewModel = new AssetBreadCrumbViewModel(ApplicationUser);
            AssetContainerContentViewModel = new AssetContainerContentViewModel(ApplicationUser);
            AssetContainerContentViewModel.OnSelection += OnSelection;
        }

        

        public override string Header => "Asset Explorer";

        public override string TabItem => "Asset Explorer";

        public override void Dispose()
        {
            ApplicationUser.OnCurrentAssetPathUpdated -= OnCurrentAssetPathUpdated;
            AssetBreadCrumbViewModel.Dispose();
            AssetContainerContentViewModel.Dispose();
        }

        public string CurrentAssetPath => ApplicationUser.CurrentAssetPath;
        private void OnCurrentAssetPathUpdated(string arg1, string arg2)
        {
            this.RaisePropertyChanged(nameof(CurrentAssetPath));
            this.RaisePropertyChanged(nameof(CanGoToParentFolder));

        }

        public bool CanGoToParentFolder => ApplicationUser.CurrentAssetContainer != null && ApplicationUser.CurrentAssetContainer is not RootAssetDirectory;

        public void GoToParentFolder()
        {
            if (ApplicationUser.CurrentAssetContainer != null && ApplicationUser.CurrentAssetContainer.ParentContainer != null)
            {
                ApplicationUser.CurrentAssetPath = ApplicationUser.CurrentAssetContainer.ParentContainer.Info.AssetPath;
            }
        }

        public void Refresh()
        {
            AssetContainerContentViewModel.Refresh();
        }

        public void AddDirectory()
        {
            if (ApplicationUser.CurrentAssetContainer != null)
            {
                this.ApplicationViewModelType.AssetManagerAvalonia.OpenCreateAssetDirectoryDialog(ApplicationUser.CurrentAssetContainer);
            }

        }

        public void AddAsset()
        {
            if (ApplicationUser.CurrentAssetContainer != null)
            {
                this.ApplicationViewModelType.AssetManagerAvalonia.OpenCreateAssetDialog(ApplicationUser.CurrentAssetContainer);
            }
        }

        public bool OpenInExplorer()
        {
            if (ApplicationUser.CurrentAssetContainer != null)
            {
                Process.Start(new ProcessStartInfo(ApplicationUser.CurrentAssetContainer.Info.FullPath) { UseShellExecute = true });
            }

            return true;
        }

        public bool CanRenameItem => AssetContainerContentViewModel.SelectedItems.Count == 1;
        public bool CanRemoveItems => AssetContainerContentViewModel.SelectedItems.Count > 0;
        private void OnSelection()
        {
            this.RaisePropertyChanged(nameof(CanRenameItem));
            this.RaisePropertyChanged(nameof(CanRemoveItems));
        }

        public void RenameItem()
        {
            AssetContainerContentViewModel.SelectedItems[0].Rename();
        }

        public void RemoveItems()
        {
            foreach (var item in AssetContainerContentViewModel.SelectedItems)
            {
                item.Remove();
            }
        }
    }
}
