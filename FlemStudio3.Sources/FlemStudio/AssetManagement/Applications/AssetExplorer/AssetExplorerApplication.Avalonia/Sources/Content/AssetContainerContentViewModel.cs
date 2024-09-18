using DynamicData;
using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class AssetContainerContentViewModel : ViewModelBase
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
            SelectedItems.Clear();
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
                    OnAssetDirectoryAdded(assetDirectory);
                }
                foreach (Asset asset in CurrentAssetContainer.EnumerateAssets())
                {
                    OnAssetAdded(asset);
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
            AssetContainerContentItemViewModel container = new AssetContainerContentItemViewModel(assetItem);
            Items.Add(container);
            container.OnSelect += OnSelectItem;
            assetItem.OnOpen += OpenAsset;
            assetItem.OnRemove += RemoveAsset;

        }

        internal List<AssetContainerContentItemViewModel> SelectedItems = new();
        public Action? OnSelection;

        public void ClearSelection()
        {
            foreach (var selectedItem in SelectedItems)
            {
                selectedItem.IsSelected = false;
            }
            SelectedItems.Clear();
            OnSelection?.Invoke();
        }
        private void OnSelectItem(AssetContainerContentItemViewModel item, bool multiple = false)
        {
            if (multiple == false)
            {
                ClearSelection();
            }
            if (item.IsSelected == false)
            {
                item.IsSelected = true;
                SelectedItems.Add(item);
            } else
            {
                if (multiple == true)
                {
                    item.IsSelected = false;
                    SelectedItems.Remove(item);
                }
                
            }
            OnSelection?.Invoke();
        }

        private void OnAssetRemoved(Asset asset)
        {
            AssetContainerContentItemViewModel? found = null;
            foreach (var item in Items)
            {
                if (item.Content is AssetItemViewModel && (item.Content as AssetItemViewModel).Asset == asset)
                {
                    found = item;
                    break;
                }
            }
            if (found != null)
            {
                Items.Remove(found);
                if (SelectedItems.Remove(found))
                {
                    OnSelection?.Invoke();
                }
            }
        }
        private void OnAssetDirectoryAdded(AssetDirectory directory)
        {
            AssetDirectoryItemViewModel directoryItem = new AssetDirectoryItemViewModel(directory);
            AssetContainerContentItemViewModel container = new AssetContainerContentItemViewModel(directoryItem);
            Items.Add(container);
            container.OnSelect += OnSelectItem;
            directoryItem.OnOpen += OpenDirectory;
            directoryItem.OnRemove += RemoveDirectory;
        }
        private void OnAssetDirectoryRemoved(AssetDirectory directory)
        {
            AssetContainerContentItemViewModel? found = null;
            foreach (var item in Items)
            {
                if (item.Content is AssetDirectoryItemViewModel && (item.Content as AssetDirectoryItemViewModel).AssetDirectory == directory)
                {
                    found = item;
                    break;
                }
            }
            if (found != null)
            {
                Items.Remove(found);
                if (SelectedItems.Remove(found))
                {
                    OnSelection?.Invoke();
                }
            }
        }

        protected void OpenDirectory(AssetDirectory assetDirectory)
        {
            ApplicationUser.CurrentAssetPath = assetDirectory.Info.AssetPath;
        }

        protected void RemoveDirectory(AssetDirectory directory)
        {
            ApplicationUser.RemoveDirectory(directory);
        }

        

        protected void OpenAsset(Asset asset)
        {
            
        }

        protected void RemoveAsset(Asset asset)
        {
            ApplicationUser.RemoveAsset(asset);
        }

        
    }
}
