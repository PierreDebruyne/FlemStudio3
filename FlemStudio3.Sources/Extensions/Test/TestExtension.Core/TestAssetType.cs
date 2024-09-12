
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.TestExtension.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TestExtension.Core
{
    public class TestAssetType : AssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public override string Description => "This is a test asset.";
        public TestAssetType() : base(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Test", "0.0.1")
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public override void OnCreateAsset(AssetInfo assetInfo)
        {
            TestAssetConfigFile configFile = new TestAssetConfigFile()
            {
                Hello = "Bonjour"
            };

            //Directory.CreateDirectory(assetInfo.FullPath);
            using (TextWriter writer = File.CreateText(assetInfo.FullPath + "/" + "Config.yaml"))
            {
                Serializer.Serialize(writer, configFile);
            }
        }

        public override void OnMoveAsset(AssetInfo assetInfo)
        {

        }
    }
}
