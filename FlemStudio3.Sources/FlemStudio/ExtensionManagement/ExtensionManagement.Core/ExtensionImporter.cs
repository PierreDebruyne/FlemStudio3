using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.ComponentModel.Composition.Hosting;

namespace FlemStudio.ExtensionManagement.Core
{
    public class ExtensionImporter
    {
        public string ExtensionFolderPath { get; }

        public string ExtensionRegistryFileName { get; } = "ExtensionRegistry.yaml";
        public string ExtensionRegistryFilePath => ExtensionFolderPath + "/" + ExtensionRegistryFileName;

        protected ExtensionRegistry ExtensionRegistry;
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }

        public AggregateCatalog Catalog { get; }
        public ExtensionImporter(string extensionFolderPath)
        {
            ExtensionFolderPath = extensionFolderPath;
            if (Directory.Exists(ExtensionFolderPath) == false)
            {
                throw new Exception("Extension directory does not exist: " + ExtensionFolderPath);
            }


            if (File.Exists(ExtensionRegistryFilePath) == false)
            {
                throw new Exception("Extension registry file does not exist: " + ExtensionRegistryFilePath);

            }
            ExtensionRegistry = ExtensionRegistry.ReadFile(ExtensionRegistryFilePath);

            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();


            Catalog = new AggregateCatalog();



            foreach (var extensionInfo in this.EnumerateExtensions())
            {

              
                if (extensionInfo.Folder != null)
                {
                    Catalog.Catalogs.Add(new DirectoryCatalog(extensionInfo.Folder));

                }

            }
        }

        public IEnumerable<IExtensionInfo> EnumerateExtensions()
        {
            foreach (ExtensionRegistryEntry entry in ExtensionRegistry.EnumerateEntries())
            {
                using (TextReader reader = File.OpenText(ExtensionFolderPath + "/" + entry.Path + "/" + "Extension.yaml"))
                {
                    ExtensionFile extensionFile = Deserializer.Deserialize<ExtensionFile>(reader);
                    yield return extensionFile;
                }
            }
        }
    }
}
