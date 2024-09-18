using System.Collections.Concurrent;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Layouts
{
    public class WindowLayoutType : LayoutType
    {
        protected ConcurrentDictionary<Guid, WindowLayout> LoadedLayouts = new();
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;
        public WindowLayoutType(LayoutService layoutService) : base(layoutService, "Window")
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public override void Update(float deltaTime)
        {
            foreach (var loadedlayout in LoadedLayouts.Values)
            {
                loadedlayout.Update(deltaTime);
            }
        }
        public override void Dispose()
        {
            foreach (var loadedlayout in LoadedLayouts.Values)
            {
                loadedlayout.Dispose();
            }
        }

        internal void OnLayoutUnused(WindowLayout loadedLayout)
        {
            LoadedLayouts.TryRemove(new KeyValuePair<Guid, WindowLayout>(loadedLayout.Guid, loadedLayout));
            loadedLayout.Dispose();
        }


        internal void SaveLayout(WindowLayout layout)
        {
            string path = GetPathFromGuid(layout.Guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, layout.Config);
            }
            Debug.WriteLine("Window layout saved: " + path);
        }


        public override LayoutUser UseLayout(Guid guid)
        {
            return UseWindowLayout(guid);
        }

        public WindowLayoutUser UseWindowLayout(Guid guid)
        {
            WindowLayout loadedLayout;
            if (LoadedLayouts.TryGetValue(guid, out loadedLayout))
            {
                return loadedLayout.CreateUser();
            }
            loadedLayout = LoadLayout(guid);
            LoadedLayouts.TryAdd(guid, loadedLayout);
            return loadedLayout.CreateUser();
        }

        public override LayoutUser UseNewLayout()
        {
            return UseNewWindowLayout();
        }

        public WindowLayoutUser UseNewWindowLayout()
        {
            Guid guid = LayoutService.Registry.AddLayout(this.Type);
            WindowLayout loadedLayout = CreateLayout(guid);
            LoadedLayouts.TryAdd(guid, loadedLayout);
            return loadedLayout.CreateUser();
        }
        protected WindowLayout LoadLayout(Guid guid)
        {
            string path = GetPathFromGuid(guid);
            WindowLayoutConfig config;
            using (TextReader reader = File.OpenText(path))
            {
                config = Deserializer.Deserialize<WindowLayoutConfig>(reader);
            }
            return new WindowLayout(this, guid, config);
        }

        protected WindowLayout CreateLayout(Guid guid)
        {
            WindowLayoutConfig config = new();
            string path = GetPathFromGuid(guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, config);
            }
            return new WindowLayout(this, guid, config);
        }

        public override void RemoveLayout(Guid guid, bool removeChildren)
        {
            if (removeChildren == true)
            {
                WindowLayoutUser windowLayout = this.UseWindowLayout(guid);
                Guid contentGuid = windowLayout.ContentLayout;
                if (contentGuid != Guid.Empty)
                {
                    LayoutService.RemoveLayout(contentGuid, removeChildren);
                }

                windowLayout.Dispose();
            }
            FileInfo fileInfo = new FileInfo(GetPathFromGuid(guid));
            fileInfo.Delete();
            LayoutService.Registry.RemoveLayout(guid);
        }
    }
    public class WindowLayoutConfig
    {
        public Guid ContentLayout { get; set; } = Guid.Empty;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
    }

    public class WindowLayout
    {
        internal WindowLayoutType LayoutType;
        public Guid Guid { get; protected set; }
        private List<WindowLayoutUser> Users = new();

        private Cooldown? SaveCooldown;

        internal WindowLayoutConfig Config;

        public WindowLayout(WindowLayoutType layoutType, Guid guid, WindowLayoutConfig config)
        {
            LayoutType = layoutType;
            Guid = guid;

            Config = config;


        }

        public void Dispose()
        {
            if (SaveCooldown != null)
            {

                Save();
            }

        }

        internal WindowLayoutUser CreateUser()
        {
            WindowLayoutUser user = new(this);
            Users.Add(user);
            return user;
        }
        internal void OnUserDisposed(WindowLayoutUser user)
        {
            Users.Remove(user);
            if (Users.Count == 0)
            {
                LayoutType.OnLayoutUnused(this);
            }
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

        public void Save()
        {

            LayoutType.SaveLayout(this);
            SaveCooldown = null;
        }

        protected void NotifyContentLayoutChanged(Guid oldContentLayout, Guid newContentLayout)
        {
            foreach (var user in new List<WindowLayoutUser>(Users))
            {
                user.OnContentLayoutChanged(oldContentLayout, newContentLayout);
            }
        }
        public Guid ContentLayout
        {
            get => Config.ContentLayout;
            set
            {
                if (Config.ContentLayout == value)
                {
                    return;
                }
                Guid oldContentLayout = Config.ContentLayout;
                Config.ContentLayout = value;
                OnUpdated();
                NotifyContentLayoutChanged(oldContentLayout, Config.ContentLayout);
            }
        }

    }

    public class WindowLayoutUser : LayoutUser
    {

        public override string LayoutType => LoadedLayout.LayoutType.Type;

        public override Guid LayoutGuid => LoadedLayout.Guid;

        protected WindowLayout LoadedLayout;

        public WindowLayoutUser(WindowLayout loadedLayout)
        {
            LoadedLayout = loadedLayout;
        }

        public override void Dispose()
        {
            LoadedLayout.OnUserDisposed(this);
        }

        public Action<Guid, Guid>? ContentLayoutChanged;

        internal void OnContentLayoutChanged(Guid oldContentLayout, Guid newContentLayout)
        {
            ContentLayoutChanged?.Invoke(oldContentLayout, newContentLayout);
        }
        public Guid ContentLayout
        {
            get => LoadedLayout.ContentLayout;
            set => LoadedLayout.ContentLayout = value;
        }
    }
}
