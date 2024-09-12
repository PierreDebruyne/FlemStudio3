using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetManagement.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.Avalonia
{
    public class AssetManagerAvalonia
    {
        protected AvaloniaApplication AvaloniaApplication;
        protected AssetManager AssetManager;

        public AssetManagerAvalonia(AvaloniaApplication avaloniaApplication, AssetManager assetManager)
        {
            AvaloniaApplication = avaloniaApplication;
            AssetManager = assetManager;
        }

        public async void OpenCreateAssetDirectoryDialog(IAssetContainer parentContainer)
        {


            CreateAssetDirectoryDialogViewModel createAssetDirectoryDialogViewModel = new CreateAssetDirectoryDialogViewModel(parentContainer);
            await AvaloniaApplication.OpenDialog(createAssetDirectoryDialogViewModel, async () =>
            {
                try
                {
                    AssetManager.CreateAssetDirectory(parentContainer.Info, createAssetDirectoryDialogViewModel.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Message: {ex.Message}");
                    Debug.WriteLine("Stacktrace:");
                    Debug.WriteLine(ex.StackTrace);
                    Debug.WriteLine("");

                    await AvaloniaApplication.OpenErrorDialog(new ErrorDialogViewModel(ex));

                }
            });
        }

        public async void OpenCreateAssetDialog(IAssetContainer parentContainer)
        {
            CreateAssetDialogViewModel createAssetDialogViewModel = new CreateAssetDialogViewModel(AssetManager);
            await AvaloniaApplication.OpenDialog(createAssetDialogViewModel, async () =>
            {
                try
                {
                    AssetManager.CreateAsset(createAssetDialogViewModel.SelectedAssetType, parentContainer.Info, createAssetDialogViewModel.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Message: {ex.Message}");
                    Debug.WriteLine("Stacktrace:");
                    Debug.WriteLine(ex.StackTrace);
                    Debug.WriteLine("");

                    await AvaloniaApplication.OpenErrorDialog(new ErrorDialogViewModel(ex));

                }
            });
        }
    }
}
