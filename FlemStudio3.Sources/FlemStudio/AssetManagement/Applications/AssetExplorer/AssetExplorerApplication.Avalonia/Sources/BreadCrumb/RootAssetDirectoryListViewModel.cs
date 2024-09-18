using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FlemStudio.AssetExplorerApplication.Avalonia
{
    public class RootAssetDirectoryListItemViewModel : ReactiveObject
    {
        internal RootAssetDirectory RootAssetDirectory;

        public RootAssetDirectoryListItemViewModel(RootAssetDirectory rootAssetDirectory)
        {
            RootAssetDirectory = rootAssetDirectory;
        }

        public string Name => RootAssetDirectory.Info.Name + ":";
    }

    public class RootAssetDirectoryListViewModel : ReactiveObject
    {
        public ObservableCollection<RootAssetDirectoryListItemViewModel> Items { get; protected set; } = new();
        public RootAssetDirectoryListItemViewModel? SelectedItem { get; protected set; }

        public RootAssetDirectoryListViewModel(IEnumerable<RootAssetDirectory> rootAssetDirectoryList)
        {

            foreach (RootAssetDirectory rootAssetDirectory in rootAssetDirectoryList)
            {
                RootAssetDirectoryListItemViewModel item = new RootAssetDirectoryListItemViewModel(rootAssetDirectory);
                Items.Add(item);
            }


        }

        public void SetSelected(RootAssetDirectory selected)
        {
            foreach (RootAssetDirectoryListItemViewModel item in Items)
            {
                if (item.RootAssetDirectory == selected)
                {
                    SelectedItem = item;
                    this.RaisePropertyChanged(nameof(SelectedItem));
                    break;
                }
            }
        }
        public Action<IAssetContainer>? OnNavigation;
        public bool HandleClick()
        {
            // Event handling logic here
            if (SelectedItem != null)
            {
                OnNavigation?.Invoke(SelectedItem.RootAssetDirectory);

            }

            return true;
        }
    }
}
