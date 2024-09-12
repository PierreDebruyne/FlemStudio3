﻿using FlemStudio.AssetManagement.Core.Assets;
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
    [AssetType(name: "ImageSource", guid: "00000000-0000-0000-0000-000000000002", version: "0.0.1")]
    public class ImageSourceAssetType2 : IAssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public string Alias => "Image source";
        public string Description => "This is an image source asset.";
        public ImageSourceAssetType2()
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

        public void OnCreateAsset(AssetInfo assetInfo)
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

        public void OnMoveAsset(AssetInfo assetInfo)
        {

        }



        public void OnDeleteAsset(AssetInfo assetInfo)
        {

        }
    }
}
