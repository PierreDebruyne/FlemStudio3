using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        public ExtensionImporter(string extensionFolderPath, IList<string> extensionNames, IList<string> contexts)
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

            foreach (string extensionName in extensionNames)
            {
                foreach (string context in contexts)
                {
                    ExtensionRegistry.TryGetEntry(extensionName + "." + context, out ExtensionRegistryEntry? entry);
                    if (entry != null)
                    {
                        aggregateCatalog.Catalogs.Add(new DirectoryCatalog(ExtensionFolderPath + "/" + entry.Path));
                    }
                }
            }

            Catalog = new FilteredCatalog(aggregateCatalog,
                def =>
                {
                    try
                    {
                        if (def.ExportDefinitions.First().Metadata.ContainsKey("Guid")
                        && def.ExportDefinitions.First().Metadata.ContainsKey("Name")
                        && def.ExportDefinitions.First().Metadata.ContainsKey("Version"))
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
