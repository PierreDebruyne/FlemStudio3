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

        public IExtensionInfo GetExtensionInfo(string extensionName)
        {
            ExtensionRegistry.TryGetEntry(extensionName, out ExtensionRegistryEntry? entry);
            if (entry == null)
            {
                throw new Exception("Extension not found: " + extensionName);
            }

            TextReader reader = File.OpenText(ExtensionFolderPath + "/" + entry.Path + "/" + "Extension.yaml");
            ExtensionFile extensionFile = Deserializer.Deserialize<ExtensionFile>(reader);
            reader.Close();

            return extensionFile;
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
                Dll_Paths = [dllPath],
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

        public void UpdateLocalExtensions()
        {
            foreach (ExtensionRegistryEntry entry in ExtensionRegistry.EnumerateEntries())
            {
                
                UpdateLocalExtension(entry.Path);
            }
        }

        public void UpdateLocalExtension(string path)
        {
            ExtensionFile extensionFile;
            using (TextReader reader = File.OpenText(ExtensionFolderPath + "/" + path + "/" + "Extension.yaml"))
            {
                extensionFile = Deserializer.Deserialize<ExtensionFile>(reader);
            }
            foreach (string dllPath in extensionFile.Dll_Paths)
            {
                FileInfo dllFileInfo = new FileInfo(dllPath);
                if (dllFileInfo.Exists)
                {
                    FileInfo destinationFileInfo = new FileInfo(ExtensionFolderPath + "/" + path + "/" + dllFileInfo.Name);
                    if (destinationFileInfo.Exists == false || destinationFileInfo.LastWriteTime != dllFileInfo.LastWriteTime)
                    {
                        dllFileInfo.CopyTo(destinationFileInfo.FullName, true);
                        Console.WriteLine(destinationFileInfo.FullName + " updated.");
                    }
                }
            }
        }


    }
}
