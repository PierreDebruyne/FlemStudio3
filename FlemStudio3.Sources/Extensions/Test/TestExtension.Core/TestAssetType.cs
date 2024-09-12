
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.TestExtension.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TestExtension.Core
{
    
    [AssetType(name: "Test", guid: "00000000-0000-0000-0000-000000000001", version: "0.0.1")]
    public class TestAssetType2 : IAssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public string Alias => "Test";
        public string Description => "This is a test asset.";
        public TestAssetType2()
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public void Test()
        {

            Console.WriteLine("YO! I'am the test asset type.");
        }

        public void OnCreateAsset(AssetInfo assetInfo)
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

        public void OnMoveAsset(AssetInfo assetInfo)
        {

        }

        

        public void OnDeleteAsset(AssetInfo assetInfo)
        {
            
        }
    }
}
