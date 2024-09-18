using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Layouts
{

    public class MainLayoutConfig
    {
        public List<Guid> Windows = new();

        public MainLayoutConfig()
        {

        }
    }
    public class MainLayout
    {
        protected string FileName;
        protected LayoutService LayoutService;
        internal MainLayoutConfig Config;

        public MainLayout(LayoutService layoutService, string fileName)
        {
            LayoutService = layoutService;
            FileName = fileName;
            if (File.Exists(FileName))
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                TextReader reader = File.OpenText(FileName);
                Config = deserializer.Deserialize<MainLayoutConfig>(reader);
                reader.Close();
            }
            else
            {
                Config = new();
                Save();
            }
        }

        public void Save()
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            TextWriter writer = File.CreateText(FileName);
            serializer.Serialize(writer, Config);
            writer.Close();
            Debug.WriteLine("Main Layout saved: " + FileName);
            SaveCooldown = null;
        }

        protected List<MainLayoutUser> Users = new();
        private Cooldown? SaveCooldown;
        public void Dispose()
        {
            if (SaveCooldown != null)
            {

                Save();
            }

        }

        internal MainLayoutUser CreateUser()
        {
            MainLayoutUser user = new(this);
            Users.Add(user);
            return user;
        }
        internal void OnUserDisposed(MainLayoutUser user)
        {
            Users.Remove(user);
        }

        protected void OnUpdated()
        {
            if (SaveCooldown == null)
            {
                SaveCooldown = new Cooldown(2.0f);
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

        public int WindowCount => Config.Windows.Count;


        public Guid GetWindowContent(int index)
        {
            if (index < 0 || index >= WindowCount)
            {
                throw new Exception("Window index is out of range: " + index);
            }
            return Config.Windows[index];
        }

        internal void AddWindow(WindowLayoutUser window)
        {
            Config.Windows.Add(window.LayoutGuid);
            OnUpdated();
            NotifyWindowAdded(window.LayoutGuid);
        }

        protected void NotifyWindowAdded(Guid guid)
        {
            foreach (var user in Users)
            {
                user.OnWindowAdded?.Invoke(guid);
            }
        }
    }

    public class MainLayoutUser : IDisposable
    {
        protected MainLayout MainLayout;

        public MainLayoutUser(MainLayout mainLayout)
        {
            MainLayout = mainLayout;
        }

        public void Dispose()
        {
            MainLayout.OnUserDisposed(this);
        }

        public int WindowCount => MainLayout.WindowCount;
        public Guid GetWindowContent(int index)
        {
            return MainLayout.GetWindowContent(index);
        }

        public void AddWindow(WindowLayoutUser window)
        {
            MainLayout.AddWindow(window);
        }

        public Action<Guid>? OnWindowAdded;
    }
}
