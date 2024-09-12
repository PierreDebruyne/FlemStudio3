using FlemStudio.AssetManagement.Core;
using FlemStudio.LayoutManagement.Core.Applications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.AssetExplorerApplication.Core
{
    public class AssetExplorerApplicationType : ApplicationType<AssetExplorerApplication, AssetExplorerApplicationUser>
    {
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;

        protected AssetManager AssetManager;

        public AssetExplorerApplicationType(AssetManager assetManager) : base("AssetExplorerApplication")
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            AssetManager = assetManager;
        }
        public override string FileExtension => Name + ".yaml";

        public override AssetExplorerApplication DoCreateApplication()
        {
            return new AssetExplorerApplication(AssetManager, new AssetExplorerApplicationState());
        }

        public override AssetExplorerApplicationUser DoCreateApplicationUser(LoadedApplication loadedApplication)
        {
            return new AssetExplorerApplicationUser(loadedApplication);
        }

        public override AssetExplorerApplication DoReadApplication(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                return new AssetExplorerApplication(AssetManager, Deserializer.Deserialize<AssetExplorerApplicationState>(reader));
            }
        }

        public override void DoWriteApplication(AssetExplorerApplication application, string path)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                Serializer.Serialize(writer, application.GetState());
            }
            Debug.WriteLine("AssetExplorer application saved: " + path);

        }
    }

    
}
