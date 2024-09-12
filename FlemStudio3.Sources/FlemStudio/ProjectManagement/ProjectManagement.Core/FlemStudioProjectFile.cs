using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.ProjectManagement.Core
{
    public class FlemStudioProjectFile
    {

        public string Name { get; set; }
        public string Version { get; set; }

        public List<string> Extensions { get; set; } = new();



        public static FlemStudioProjectFile ReadFile(string path)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextReader reader = File.OpenText(path);
            FlemStudioProjectFile projectFile = deserializer.Deserialize<FlemStudioProjectFile>(reader);
            reader.Close();

            return projectFile;
        }

        public static void WriteFile(string path, FlemStudioProjectFile projectFile)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextWriter writer = File.CreateText(path);
            serializer.Serialize(writer, projectFile);
            writer.Close();
            Debug.WriteLine("Project file saved: " + path);
        }
    }
}
