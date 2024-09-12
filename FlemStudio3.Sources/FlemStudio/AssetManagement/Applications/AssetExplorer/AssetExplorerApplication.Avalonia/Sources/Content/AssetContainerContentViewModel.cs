using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public abstract class AssetContainerContentItemViewModel: ReactiveObject
    {
        public abstract void Dispose();
    }

    public class AssetContainerContentViewModel : ReactiveObject
    {

        protected AssetExplorerApplicationUser ApplicationUser;

        protected IAssetContainer? CurrentAssetContainer;

        public ObservableCollection<AssetContainerContentItemViewModel> Items { get; } = new();
        
        public AssetContainerContentViewModel(AssetExplorerApplicationUser applicationUser)
        {
            ApplicationUser = applicationUser;
            ApplicationUser.OnCurrentAssetPathUpdated += OnCurrentAssetPathUpdated;
            Refresh();
        }

        

        public void Dispose()
        {
            ApplicationUser.OnCurrentAssetPathUpdated -= OnCurrentAssetPathUpdated;
            if (CurrentAssetContainer != null)
            {
                CurrentAssetContainer.OnAssetAdded -= OnAssetAdded;
                CurrentAssetContainer.OnAssetRemoved -= OnAssetRemoved;
                CurrentAssetContainer.OnAssetDirectoryAdded -= OnAssetDirectoryAdded;
                CurrentAssetContainer.OnAssetDirectoryRemoved -= OnAssetDirectoryRemoved;
            }
        }

        private void OnCurrentAssetPathUpdated(string arg1, string arg2)
        {

            Refresh();
        }

        public void Refresh()
        {
            if (CurrentAssetContainer != null)
            {
                CurrentAssetContainer.OnAssetAdded -= OnAssetAdded;
                CurrentAssetContainer.OnAssetRemoved -= OnAssetRemoved;
                CurrentAssetContainer.OnAssetDirectoryAdded -= OnAssetDirectoryAdded;
                CurrentAssetContainer.OnAssetDirectoryRemoved -= OnAssetDirectoryRemoved;
            }
            CurrentAssetContainer = ApplicationUser.CurrentAssetContainer;


            foreach (AssetContainerContentItemViewModel item in Items)
            {
                item.Dispose();
            }
            Items.Clear();
            if (CurrentAssetContainer != null)
            {

                foreach (AssetDirectory assetDirectory in CurrentAssetContainer.EnumerateDirectories())
                {
                    AssetDirectoryItemViewModel directoryItem = new AssetDirectoryItemViewModel(assetDirectory);
                    Items.Add(directoryItem);
                    directoryItem.OnOpen += OpenDirectory;
                }
                foreach (Asset asset in CurrentAssetContainer.EnumerateAssets())
                {
                    AssetItemViewModel assetItem = new AssetItemViewModel(asset);
                    Items.Add(assetItem);
                }

                CurrentAssetContainer.OnAssetAdded += OnAssetAdded;
                CurrentAssetContainer.OnAssetRemoved += OnAssetRemoved;
                CurrentAssetContainer.OnAssetDirectoryAdded += OnAssetDirectoryAdded;
                CurrentAssetContainer.OnAssetDirectoryRemoved += OnAssetDirectoryRemoved;
            }
        }
       
        private void OnAssetAdded(Asset asset)
        {
            AssetItemViewModel assetItem = new AssetItemViewModel(asset);
            Items.Add(assetItem);
        }
        private void OnAssetRemoved(Asset asset)
        {
            AssetContainerContentItemViewModel? found = null;
            foreach (var item in Items)
            {
                if (item is AssetItemViewModel && (item as AssetItemViewModel).Asset == asset)
                {
                    found = item;
                    break;
                }
            }
            if (found != null)
            {
                Items.Remove(found);
            }
        }
        private void OnAssetDirectoryAdded(AssetDirectory directory)
        {
            AssetDirectoryItemViewModel directoryItem = new AssetDirectoryItemViewModel(directory);
            Items.Add(directoryItem);
            directoryItem.OnOpen += OpenDirectory;
        }
        private void OnAssetDirectoryRemoved(AssetDirectory directory)
        {
            AssetContainerContentItemViewModel? found = null;
            foreach (var item in Items)
            {
                if (item is AssetDirectoryItemViewModel && (item as AssetDirectoryItemViewModel).AssetDirectory == directory)
                {
                    found = item;
                    break;
                }
            }
            if (found != null)
            {
                Items.Remove(found);
            }
        }

        protected void OpenDirectory(AssetDirectory assetDirectory)
        {
            ApplicationUser.CurrentAssetPath = assetDirectory.Info.AssetPath;
        }

        
    }
}
