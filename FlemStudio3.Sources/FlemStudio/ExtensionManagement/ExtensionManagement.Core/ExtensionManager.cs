using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.ExtensionManagement.Core
{

    public class ExtensionManager
    {
        public string ExtensionFolderPath { get; }

        public string ExtensionRegistryFileName { get; } = "ExtensionRegistry.yaml";
        public string ExtensionRegistryFilePath => ExtensionFolderPath + "/" + ExtensionRegistryFileName;

        protected ExtensionRegistry ExtensionRegistry;
        public ISerializer Serializer { get; }
        public IDeserializer Deserializer { get; }





        public ExtensionManager(string extensionFolderPath)
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
        }

        public ExtensionInfo GetExtensionInfo(string extensionName)
        {
            ExtensionRegistry.TryGetEntry(extensionName, out ExtensionRegistryEntry? entry);
            if (entry == null)
            {
                throw new Exception("Extension not found: " + extensionName);
            }

            TextReader reader = File.OpenText(ExtensionFolderPath + "/" + entry.Path + "/" + "Extension.yaml");
            ExtensionFile extensionFile = Deserializer.Deserialize<ExtensionFile>(reader);
            reader.Close();

            ExtensionInfo extensionInfo = new ExtensionInfo(this, extensionFile.Guid, extensionFile.Name, extensionFile.Version, extensionFile.Dll_Path);
            return extensionInfo;
        }

        public void CreateExtension(string extensionName, string dllPath)
        {
            ExtensionRegistry.TryGetEntry(extensionName, out ExtensionRegistryEntry? entry);
            if (entry != null)
            {
                throw new Exception("This extension already exist: " + extensionName);
            }
            string folderPath = ExtensionFolderPath + "/" + extensionName;
            if (Directory.Exists(folderPath))
            {
                throw new Exception("This extension folder already exist: " + folderPath);
            }
            ExtensionFile extensionFile = new ExtensionFile()
            {
                Guid = Guid.NewGuid(),
                Name = extensionName,
                Version = "0.0.1",
                Dll_Path = dllPath,
            };
            Directory.CreateDirectory(folderPath);
            TextWriter writer = File.CreateText(folderPath + "/" + "Extension.yaml");
            Serializer.Serialize(writer, extensionFile);
            writer.Close();
            Console.WriteLine("Extension created: " + extensionName);
            ExtensionRegistry.AddEntry(new ExtensionRegistryEntry()
            {
                Name = extensionName,
                Guid = extensionFile.Guid,
                Path = extensionName,
            });
            ExtensionRegistry.WriteFile(ExtensionRegistryFilePath, ExtensionRegistry);

        }


    }
}
