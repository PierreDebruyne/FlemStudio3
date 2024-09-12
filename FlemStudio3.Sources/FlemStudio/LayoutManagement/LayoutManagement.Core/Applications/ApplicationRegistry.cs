using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Applications
{

    public class ApplicationRegistryItem
    {
        public Guid Guid { get; set; }
        public string Type { get; set; } = "Unknown";
        public string? Tag { get; set; }
    }

    public class ApplicationRegistry
    {

        public string RegistryFileName { get; protected set; }
        protected Cooldown? SaveCooldown;

        protected Dictionary<Guid, ApplicationRegistryItem> Applications = new();
        protected Dictionary<string, ApplicationRegistryItem> ApplicationsByTag = new();

        public ApplicationRegistry(string registryFileName)
        {
            RegistryFileName = registryFileName;
            if (File.Exists(RegistryFileName))
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                TextReader reader = File.OpenText(RegistryFileName);
                List<ApplicationRegistryItem> entries = deserializer.Deserialize<List<ApplicationRegistryItem>>(reader);
                foreach (var entry in entries)
                {
                    Applications.Add(entry.Guid, entry);
                    if (entry.Tag != null)
                    {
                        ApplicationsByTag.Add(entry.Tag, entry);
                    }
                }
                reader.Close();
            }
            else
            {
                
                Save();
            }
        }

        public void Save()
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextWriter writer = File.CreateText(RegistryFileName);
            serializer.Serialize(writer, Applications.Values);
            writer.Close();
            Debug.WriteLine("Application registry saved: " + RegistryFileName);
            SaveCooldown = null;
        }


        protected void OnUpdated()
        {
            if (SaveCooldown == null)
            {
                SaveCooldown = new Cooldown(1.0f);
            }
            else
            {
                SaveCooldown.Reset();
            }
        }
        public void Update(float deltaTime)
        {
            if (SaveCooldown != null)
            {
                if (SaveCooldown.Update(deltaTime) == true)
                {
                    Save();
                }
            }
        }

        public void Dispose()
        {
            if (SaveCooldown != null)
            {

                Save();
            }
        }

        
        public void AddApplication(Guid guid, string type, string? tag)
        {
            ApplicationRegistryItem entry = new ApplicationRegistryItem()
            {
                Guid = guid,
                Type = type,
                Tag = tag,
            };
            Applications.Add(guid, entry);
            if (tag != null)
            {
                ApplicationsByTag.Add(tag, entry);
            }
            OnUpdated();
        }
        

        public void RemoveApplication(Guid guid)
        {
            ApplicationRegistryItem entry = GetApplicationEntry(guid);
            Applications.Remove(guid);
            if (entry.Tag != null)
            {
                ApplicationsByTag.Remove(entry.Tag);
            }
            OnUpdated();
        }

        public bool TryGetApplicationEntry(Guid guid, out ApplicationRegistryItem? entry)
        {
            if (Applications.TryGetValue(guid, out ApplicationRegistryItem? item))
            {
                entry = item;
                return true;
            }
            else
            {
                entry = null;
                return false;
            }
        }

        public ApplicationRegistryItem GetApplicationEntry(Guid guid)
        {
            if (TryGetApplicationEntry(guid, out ApplicationRegistryItem? entry))
            {
                return entry;
            }
            else
            {
                throw new Exception("This application is not in the registry: " + guid);
            }
        }

        public Guid NewGuid()
        {
            Guid guid = Guid.NewGuid();
            while (this.Applications.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }
            return guid;
        }

        public bool ContainTag(string tag)
        {
            return ApplicationsByTag.ContainsKey(tag);
        }
    }
}
