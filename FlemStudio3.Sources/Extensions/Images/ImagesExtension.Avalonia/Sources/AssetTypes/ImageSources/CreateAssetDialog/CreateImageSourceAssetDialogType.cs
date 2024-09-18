using FlemStudio.AssetManagement.Avalonia;
using FlemStudio.AssetManagement.Core.Assets;
using ImagesExtension.Core;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace ImagesExtension.Avalonia
{
    [CreateAssetDialogType(name: "ImageSource", guid: "00000000-0000-0000-0000-000000000002", version: "0.0.1")]
    public class CreateImageSourceAssetDialogType : CreateAssetDialogType<CreateImageSourceAssetDialogViewModel>
    {

        [Import(typeof(ImageSourceAssetType), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ImageSourceAssetType ImageSourceAssetType { get; set; }


        public override CreateImageSourceAssetDialogViewModel DoCreateDialogViewModel()
        {
            return new CreateImageSourceAssetDialogViewModel();
        }

        public override void DoCreateAsset(AssetInfo assetInfo, CreateImageSourceAssetDialogViewModel dialogViewModel)
        {
            if (dialogViewModel.ImagePath.Length > 0)
            {
                ImageSourceAssetType.OnCreateAsset(assetInfo, dialogViewModel.ImagePath);
            }
            else
            {
                ImageSourceAssetType.OnCreateAssetDefault(assetInfo);
            }
        }



        public override void Test()
        {
            Debug.WriteLine("YO, I'am the CreateImageSourceAssetDialogType Avalonia!");
        }
    }
}
