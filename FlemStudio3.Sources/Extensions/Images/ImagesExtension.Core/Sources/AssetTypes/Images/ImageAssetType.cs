using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using ImagesExtension.Core.Properties;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace ImagesExtension.Core.AssetTypes.Images
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ImageAssetType))]
    [AssetType(name: "Image", guid: "00000000-0000-0000-0000-000000000003", version: "0.0.1")]
    public class ImageAssetType : IAssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public string Alias => "Image";
        public string Description => "This is an image asset.";
        public ImageAssetType()
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).DisableAliases().Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public void Test()
        {
            Console.WriteLine("YO! I'am the image asset type.");
            Image<Rgba32> image = Image.Load<Rgba32>(Resources.grid);
            image.Dispose();
        }

        public void OnCreateAssetDefault(AssetInfo assetInfo)
        {
            ImageAssetConfigFile configFile = new ImageAssetConfigFile()
            {
                
            };

            using (TextWriter writer = File.CreateText(assetInfo.FullPath + "/" + "Config.yaml"))
            {
                Serializer.Serialize(writer, configFile);
            }

           
        }

       

        public void OnMoveAsset(AssetInfo assetInfo)
        {

        }



        public void OnDeleteAsset(AssetInfo assetInfo)
        {

        }
    }
}
