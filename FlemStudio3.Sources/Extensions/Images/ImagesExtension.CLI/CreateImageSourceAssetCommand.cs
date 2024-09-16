using FlemStudio.AssetManagement.CLI.Assets.Create;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using ImagesExtension.Core;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel.Composition;

namespace ImagesExtension.CLI
{
    [CreateAssetCommandType(name: "ImageSource", guid: "00000000-0000-0000-0000-000000000002", version: "0.0.1")]
    public class CreateImageSourceAssetCommand : ICreateAssetCommandType
    {

        [Import(typeof(ImageSourceAssetType), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ImageSourceAssetType ImageSourceAssetType { get; set; }

        protected Option<string?> ImageOption;

        public void Test()
        {
            if (ImageSourceAssetType != null)
            {
                Console.WriteLine("YO, I'am the Create Image source asset command.");
                ImageSourceAssetType.Test();
            }
            else
            {
                Console.WriteLine("BUG, I'am the Create Image source asset command.");
            }
        }

        public void ModifyCommand(Command command)
        {
            ImageOption = new Option<string?>(
               name: "--source",
               description: "The image source file."
               );
            command.AddOption(ImageOption);
        }

        public void OnCreateAsset(AssetInfo assetInfo, InvocationContext context)
        {
            string? imagePath = context.ParseResult.GetValueForOption(ImageOption);
            if (imagePath == null)
            {
                ImageSourceAssetType.OnCreateAssetDefault(assetInfo);
            } else
            {
                ImageSourceAssetType.OnCreateAsset(assetInfo, imagePath);
            }
        }

        
    }
}
