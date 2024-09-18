using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.ExtensionManagement.Core;
using System.Diagnostics;

namespace FlemStudio.AssetManagement.Avalonia
{
    public class AssetManagerAvalonia
    {
        protected AvaloniaApplication AvaloniaApplication;
        protected AssetManager AssetManager;
        protected CreateAssetDialogRegistry CreateAssetDialogRegistry;

        public AssetManagerAvalonia(AvaloniaApplication avaloniaApplication, AssetManager assetManager, ExtensionImporter extensionImporter)
        {
            AvaloniaApplication = avaloniaApplication;
            AssetManager = assetManager;

            CreateAssetDialogRegistry = new CreateAssetDialogRegistry(assetManager);
            CreateAssetDialogRegistry.LoadExtensions(extensionImporter);
            CreateAssetDialogRegistry.TestExtensions();
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
                    CreateAssetDialogRegistry.TryGetCreateAssetDialogType(createAssetDialogViewModel.SelectedAssetType.Guid, out CreateAssetDialogTypeDefinition? createAssetDialogType);

                    if (createAssetDialogType == null)
                    {
                        AssetManager.CreateAsset(createAssetDialogViewModel.SelectedAssetType, parentContainer.Info, createAssetDialogViewModel.Name);
                    }
                    else
                    {
                        DialogViewModel createTypeDialogViewModel = createAssetDialogType.CreateAssetDialogType.CreateDialogViewModel();
                        await AvaloniaApplication.OpenDialog(createTypeDialogViewModel, async () =>
                        {
                            try
                            {
                                AssetManager.CreateAsset(createAssetDialogViewModel.SelectedAssetType, parentContainer.Info, createAssetDialogViewModel.Name, (AssetInfo assetInfo) =>
                                {
                                    createAssetDialogType.CreateAssetDialogType.OnCreateAsset(assetInfo, createTypeDialogViewModel);
                                });
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
