using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

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

        protected ComposablePartCatalog Catalog;
        public CompositionContainer CompositionContainer { get; }
        public ExtensionImporter(string extensionFolderPath, IList<Guid> extensionGuids)
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


            AggregateCatalog aggregateCatalog = new AggregateCatalog();

            foreach (ExtensionRegistryEntry entry in ExtensionRegistry.EnumerateEntries())
            {
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(ExtensionFolderPath + "/" + entry.Path));
                Console.WriteLine(ExtensionFolderPath + "/" + entry.Path);
                
            }

            Catalog = new FilteredCatalog(aggregateCatalog,
                def => {

                    try
                    {
                        if (def.ExportDefinitions.First().Metadata.ContainsKey("Guid")
                                                && extensionGuids.Contains(Guid.Parse((string)def.ExportDefinitions.First().Metadata["Guid"])))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }

                    return false;
                });
            CompositionContainer = new CompositionContainer(Catalog);

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
