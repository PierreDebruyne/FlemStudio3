﻿using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using FlemStudio.LayoutManagement.Avalonia.Applications;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
