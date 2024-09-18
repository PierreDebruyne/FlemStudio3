using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using ImagesExtension.Core.Properties;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel.Composition;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImagesExtension.Core
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ImageSourceAssetType))]
    [AssetType(name: "ImageSource", guid: "00000000-0000-0000-0000-000000000002", version: "0.0.1")]
    public class ImageSourceAssetType : IAssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public string Alias => "Image source";
        public string Description => "This is an image source asset.";
        public ImageSourceAssetType()
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public void Test()
        {
            Console.WriteLine("YO! I'am the image source asset type.");
            Image<Rgba32> image = Image.Load<Rgba32>(Resources.grid);
            image.Dispose();
        }

        public void OnCreateAssetDefault(AssetInfo assetInfo)
        {
            ImageSourceAssetConfigFile configFile = new ImageSourceAssetConfigFile()
            {
                ImagePath = "grid.png"
            };

            using (TextWriter writer = File.CreateText(assetInfo.FullPath + "/" + "Config.yaml"))
            {
                Serializer.Serialize(writer, configFile);
            }

            Image<Rgba32> image = Image.Load<Rgba32>(Resources.grid);
            image.Save(assetInfo.FullPath + "/" + "grid.png");
            image.Dispose();
        }

        public void OnCreateAsset(AssetInfo assetInfo, string imagePath)
        {

            FileInfo imageFileInfo = new FileInfo(imagePath);

            ImageSourceAssetConfigFile configFile = new ImageSourceAssetConfigFile()
            {
                ImagePath = imageFileInfo.Name
            };

            using (TextWriter writer = File.CreateText(assetInfo.FullPath + "/" + "Config.yaml"))
            {
                Serializer.Serialize(writer, configFile);
            }


            if (imageFileInfo.Exists == false)
            {
                throw new Exception("Image not found: " + imagePath);
            }
            Image<Rgba32> image = Image.Load<Rgba32>(imagePath);
            image.Save(assetInfo.FullPath + "/" + imageFileInfo.Name);
        }

        public void OnMoveAsset(AssetInfo assetInfo)
        {

        }



        public void OnDeleteAsset(AssetInfo assetInfo)
        {

        }
    }
}
