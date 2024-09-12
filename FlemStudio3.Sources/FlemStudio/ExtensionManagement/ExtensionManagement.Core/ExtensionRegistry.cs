using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.ExtensionManagement.Core
{
    public class ExtensionRegistryEntry
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
    public class ExtensionRegistry
    {
        protected Dictionary<Guid, ExtensionRegistryEntry> EntriesByGuid = new();
        protected Dictionary<string, ExtensionRegistryEntry> EntriesByName = new();

        public bool TryGetEntry(Guid guid, out ExtensionRegistryEntry? entry)
        {
            return EntriesByGuid.TryGetValue(guid, out entry);
        }

        public bool TryGetEntry(string name, out ExtensionRegistryEntry? entry)
        {
            return EntriesByName.TryGetValue(name, out entry);
        }

        public void AddEntry(ExtensionRegistryEntry entry)
        {
            EntriesByGuid.Add(entry.Guid, entry);
            EntriesByName.Add(entry.Name, entry);
        }

        public IEnumerable<ExtensionRegistryEntry> EnumerateEntries()
        {
            foreach (ExtensionRegistryEntry entry in EntriesByGuid.Values)
            {
                yield return entry;
            }
        }

        public static ExtensionRegistry ReadFile(string path)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextReader reader = File.OpenText(path);
            IList<ExtensionRegistryEntry>? entries = deserializer.Deserialize<IList<ExtensionRegistryEntry>?>(reader);
            reader.Close();
            ExtensionRegistry registry = new ExtensionRegistry();
            if (entries != null)
            {
                foreach (ExtensionRegistryEntry entry in entries)
                {
                    registry.AddEntry(entry);
                }
            }

            return registry;
        }

        public static void WriteFile(string path, ExtensionRegistry regisry)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextWriter writer = File.CreateText(path);
            serializer.Serialize(writer, regisry.EntriesByGuid.Values);
            writer.Close();
            Debug.WriteLine("Extension registry saved: " + path);
        }
    }
}
