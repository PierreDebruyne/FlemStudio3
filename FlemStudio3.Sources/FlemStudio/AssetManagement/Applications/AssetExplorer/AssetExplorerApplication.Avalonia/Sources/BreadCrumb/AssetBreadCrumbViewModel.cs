using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetBreadCrumbItemViewModel : ReactiveObject
    {
        protected AssetBreadCrumbViewModel BreadCrumbViewModel;
        protected IAssetContainer AssetContainer;


        public AssetBreadCrumbItemViewModel(AssetBreadCrumbViewModel breadCrumbViewModel, IAssetContainer assetContainer)
        {
            BreadCrumbViewModel = breadCrumbViewModel;
            AssetContainer = assetContainer;
            Init();
        }

        protected void Init()
        {

        }

        public void Dispose()
        {
            
        }

        public string Name => AssetContainer.Info.Name;

        public bool HandleClick()
        {
            // Event handling logic here
            BreadCrumbViewModel.OnItemClick(AssetContainer);
            return true;
        }
    }
    public class AssetBreadCrumbViewModel : ReactiveObject
    {

        protected AssetExplorerApplicationUser ApplicationUser;

        protected IAssetContainer? CurrentAssetContainer => ApplicationUser.CurrentAssetContainer;
        public RootAssetDirectoryListViewModel RootAssetDirectoryListViewModel { get; }
        public ObservableCollection<AssetBreadCrumbItemViewModel> PathList { get; protected set; }


        public AssetBreadCrumbViewModel(AssetExplorerApplicationUser applicationUser)
        {
            ApplicationUser = applicationUser;
            ApplicationUser.OnCurrentAssetPathUpdated += OnCurrentAssetPathUpdated;
            RootAssetDirectoryListViewModel = new RootAssetDirectoryListViewModel(ApplicationUser.EnumerateRootAssetDirectory());
            RootAssetDirectoryListViewModel.OnNavigation += OnNavigation;
            Init();
        }

        private void OnNavigation(IAssetContainer container)
        {
            if  (ApplicationUser.CurrentAssetContainer == null || ApplicationUser.CurrentAssetContainer != container)
            {
                ApplicationUser.CurrentAssetPath = container.Info.AssetPath;
            }
        }

        protected void Init()
        {
            PathList = new ObservableCollection<AssetBreadCrumbItemViewModel>();
            UpdatePathList();
        }

        public void OnCurrentAssetPathUpdated(string oldValue, string newValue)
        {
            UpdatePathList();
        }

       



        protected void UpdatePathList()
        {
            foreach (var item in PathList)
            {
                item.Dispose();
            }
            PathList.Clear();

            IAssetContainer? container = CurrentAssetContainer;
            while (container != null && container is AssetDirectory) {


                PathList.Insert(0, new AssetBreadCrumbItemViewModel(this, container));
                container = container.ParentContainer;
            }
            if (container != null && container is RootAssetDirectory)
            {
                RootAssetDirectoryListViewModel.SetSelected((RootAssetDirectory)container);
            }

            
            
        }

        internal void OnItemClick(IAssetContainer assetContainer)
        {
            OnNavigation(assetContainer);
        }

        internal void Dispose()
        {
            foreach (var item in PathList)
            {
                item.Dispose();
            }
            PathList.Clear();
            ApplicationUser.OnCurrentAssetPathUpdated -= OnCurrentAssetPathUpdated;
            
        }
    }
}
