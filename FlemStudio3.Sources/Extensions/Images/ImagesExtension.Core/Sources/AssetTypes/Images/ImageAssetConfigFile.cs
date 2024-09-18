using FlemStudio.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesExtension.Core.AssetTypes.Images
{
    public interface IImageAssetConfig
    {
        public Guid ImageSourceAsset { get; }

        public Vector2i SourceOffset { get; }
        public Vector2i SourceSize { get; }

        public Vector2i TargetSize { get; }
        public string BackgroundColor { get; }
    }

    public class ImageAssetConfigFile : IImageAssetConfig
    {
        public Guid ImageSourceAsset { get; set; }

        public Vector2i SourceOffset { get; set; }
        public Vector2i SourceSize { get; set; }

        public Vector2i TargetSize { get; set; }

        public string BackgroundColor { get; set; } = "#00000000";
    }
    
}
