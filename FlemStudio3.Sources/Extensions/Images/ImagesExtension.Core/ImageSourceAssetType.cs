using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using ImagesExtension.Core.Properties;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImagesExtension.Core
{

    public class ImageSourceAssetType : AssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public override string Description => "This is an image source asset.";
        public ImageSourceAssetType() : base(Guid.Parse("00000000-0000-0000-0000-000000000002"), "ImageSource", "0.0.1")
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public override void OnCreateAsset(AssetInfo assetInfo)
        {
            ImageSourceAssetConfigFile configFile = new ImageSourceAssetConfigFile()
            {
                ImagePath = "grid.png"
            };

            //Directory.CreateDirectory(assetInfo.FullPath);
            using (TextWriter writer = File.CreateText(assetInfo.FullPath + "/" + "Config.yaml"))
            {
                Serializer.Serialize(writer, configFile);
            }

            Image<Rgba32> image = Image.Load<Rgba32>(Resources.grid);
            image.Save(assetInfo.FullPath + "/" + "grid.png");
            image.Dispose();
        }

        public override void OnMoveAsset(AssetInfo assetInfo)
        {

        }
    }
}
