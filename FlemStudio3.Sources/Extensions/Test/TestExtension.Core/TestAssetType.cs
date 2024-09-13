
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.TestExtension.Core;
using System.ComponentModel.Composition;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TestExtension.Core
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(TestAssetType))]
    
    [AssetType(name: "Test", guid: "00000000-0000-0000-0000-000000000001", version: "0.0.1")]
    public class TestAssetType : IAssetType
    {
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public string Alias => "Test";
        public string Description => "This is a test asset.";
        public TestAssetType()
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Console.WriteLine("CREATED! I'am the test asset type.");
        }

        public void Test()
        {

            Console.WriteLine("YO! I'am the test asset type.");
        }

        public void OnCreateAssetDefault(AssetInfo assetInfo)
        {
            OnCreateAsset(assetInfo, "Bonjour");
        }

        public void OnCreateAsset(AssetInfo assetInfo, string hello)
        {
            TestAssetConfigFile configFile = new TestAssetConfigFile()
            {
                Hello = hello
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
