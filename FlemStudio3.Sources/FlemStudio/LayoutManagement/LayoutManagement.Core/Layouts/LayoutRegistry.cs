
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Layouts
{
    public class LayoutRegistryEntry
    {
        public string Type { get; set; } = "Unknown";

    }
    public class LayoutRegistry
    {
        public string RegistryFileName { get; protected set; }
        protected Cooldown? SaveCooldown;
        protected Dictionary<Guid, LayoutRegistryEntry> Entries;


        public bool IsUpdated { get; protected set; } = false;

        public LayoutRegistry(string registryFileName)
        {
            RegistryFileName = registryFileName;
            if (File.Exists(RegistryFileName))
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                TextReader reader = File.OpenText(RegistryFileName);
                Entries = deserializer.Deserialize<Dictionary<Guid, LayoutRegistryEntry>>(reader);
                reader.Close();
            }
            else
            {
                Entries = new();
                Save();
            }
        }

        public void Save()
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextWriter writer = File.CreateText(RegistryFileName);
            serializer.Serialize(writer, Entries);
            writer.Close();
            Debug.WriteLine("Layout registry saved: " + RegistryFileName);
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

        public Guid NewGuid()
        {
            Guid guid = Guid.NewGuid();
            while (Entries.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }
            return guid;
        }

        public Guid AddLayout(string type)
        {
            Guid guid = NewGuid();

            Entries.Add(guid, new LayoutRegistryEntry()
            {
                Type = type
            });
            OnUpdated();
            return guid;
        }

        public void RemoveLayout(Guid guid)
        {
            if (Entries.Remove(guid))
            {
                OnUpdated();
            }
            else
            {
                throw new Exception("This layout is not in the registry: " + guid);
            }

        }

        public bool TryGetLayoutType(Guid guid, out string type)
        {
            if (Entries.TryGetValue(guid, out LayoutRegistryEntry? entry))
            {
                type = entry.Type;
                return true;
            }
            else
            {
                type = "Unknown";
                return false;
            }
        }

        public string GetLayoutType(Guid guid)
        {
            if (TryGetLayoutType(guid, out string type))
            {
                return type;
            }
            else
            {
                throw new Exception("This layout is not in the registry: " + guid);
            }
        }


    }
}
