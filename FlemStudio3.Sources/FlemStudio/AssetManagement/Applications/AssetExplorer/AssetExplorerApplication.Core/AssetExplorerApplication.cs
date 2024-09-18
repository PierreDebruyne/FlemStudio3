using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using FlemStudio.LayoutManagement.Core.Applications;

namespace FlemStudio.AssetExplorerApplication.Core
{
    public interface IAssetExplorerApplication
    {
        public Action<string, string>? OnCurrentAssetPathUpdated { get; set; }
        public string CurrentAssetPath { get; set; }
        public IAssetContainer? CurrentAssetContainer { get; }
        public IEnumerable<RootAssetDirectory> EnumerateRootAssetDirectory();
    }
    public class AssetExplorerApplicationState
    {
        public string CurrentAssetPath = "Local:/";
    }
    public class AssetExplorerApplication : Application<AssetExplorerApplicationState>, IAssetExplorerApplication
    {
        protected AssetManager AssetManager;

        public IAssetContainer? CurrentAssetContainer { get; protected set; }
        public AssetExplorerApplication(AssetManager assetManager, AssetExplorerApplicationState state) : base(state)
        {
            AssetManager = assetManager;
            UpdateAssetContainer();
        }

        public override void Dispose()
        {

        }

        public Action<string, string>? OnCurrentAssetPathUpdated { get; set; }
        public string CurrentAssetPath
        {
            get
            {
                return State.CurrentAssetPath;
            }
            set
            {
                if (State.CurrentAssetPath != value)
                {
                    string oldValue = State.CurrentAssetPath;
                    State.CurrentAssetPath = value;
                    OnStateUpdated?.Invoke();
                    UpdateAssetContainer();
                    OnCurrentAssetPathUpdated?.Invoke(oldValue, value);
                }
            }
        }

        protected void UpdateAssetContainer()
        {
            AssetManager.AssetRegistry.TryGetAssetContainer(CurrentAssetPath, out IAssetContainer? currentAssetContainer);
            CurrentAssetContainer = currentAssetContainer;
        }

        public IEnumerable<RootAssetDirectory> EnumerateRootAssetDirectory()
        {
            return AssetManager.AssetRegistry.EnumerateRootAssetDirectory();
        }

        internal void RemoveDirectory(AssetDirectory directory)
        {
            AssetManager.RemoveAssetDirectory(directory, true);
        }

        internal void RemoveAsset(Asset asset)
        {
            AssetManager.RemoveAsset(asset);
        }
    }

    public class AssetExplorerApplicationUser : ApplicationUser<AssetExplorerApplication>, IAssetExplorerApplication
    {
        public AssetExplorerApplicationUser(LoadedApplication loadedApplication) : base(loadedApplication)
        {
            Application.OnCurrentAssetPathUpdated += NotifyCurrentAssetPathUpdated;
        }

        public override void Dispose()
        {
            Application.OnCurrentAssetPathUpdated -= NotifyCurrentAssetPathUpdated;
            base.Dispose();
        }

        public void NotifyCurrentAssetPathUpdated(string oldValue, string newValue)
        {
            OnCurrentAssetPathUpdated?.Invoke(oldValue, newValue);
        }



        public Action<string, string>? OnCurrentAssetPathUpdated { get; set; }
        public string CurrentAssetPath
        {
            get => Application.CurrentAssetPath;
            set => Application.CurrentAssetPath = value;
        }

        public IAssetContainer? CurrentAssetContainer => Application.CurrentAssetContainer;

        public IEnumerable<RootAssetDirectory> EnumerateRootAssetDirectory()
        {
            return Application.EnumerateRootAssetDirectory();
        }

        public void RemoveDirectory(AssetDirectory directory)
        {
            Application.RemoveDirectory(directory);
        }

        public void RemoveAsset(Asset asset)
        {
            Application.RemoveAsset(asset);
        }

    }
}
