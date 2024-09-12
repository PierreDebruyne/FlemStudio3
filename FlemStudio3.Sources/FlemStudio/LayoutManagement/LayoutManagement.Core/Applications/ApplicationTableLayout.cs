using FlemStudio.LayoutManagement.Core.Layouts;
using System.Collections.Concurrent;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Applications
{

    public class ApplicationTableLayoutType : LayoutType
    {
        internal ApplicationService ApplicationService;
        protected ConcurrentDictionary<Guid, ApplicationTableLayout> LoadedLayouts = new();
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;

        public ApplicationTableLayoutType(LayoutService layoutService, ApplicationService applicationService) : base(layoutService, "ApplicationTable")
        {
            ApplicationService = applicationService;
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

        internal void OnLayoutUnused(ApplicationTableLayout loadedLayout)
        {
            LoadedLayouts.TryRemove(new KeyValuePair<Guid, ApplicationTableLayout>(loadedLayout.Guid, loadedLayout));
            loadedLayout.Dispose();
        }

        
        internal void SaveLayout(ApplicationTableLayout layout)
        {
            string path = GetPathFromGuid(layout.Guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, layout.Config);
            }
            Debug.WriteLine("Tab layout saved: " + path);
        }


        public override LayoutUser UseLayout(Guid guid)
        {
            return UseTabLayout(guid);
        }

        public ApplicationTableLayoutUser UseTabLayout(Guid guid)
        {
            ApplicationTableLayout loadedLayout;
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
            return UseNewTabLayout();
        }

        public ApplicationTableLayoutUser UseNewTabLayout()
        {
            Guid guid = LayoutService.Registry.AddLayout(this.Type);
            ApplicationTableLayout loadedLayout = CreateLayout(guid);
            LoadedLayouts.TryAdd(guid, loadedLayout);
            return loadedLayout.CreateUser();
        }
        protected ApplicationTableLayout LoadLayout(Guid guid)
        {
            string path = GetPathFromGuid(guid);
            ApplicationTableLayoutConfig config;
            using (TextReader reader = File.OpenText(path))
            {
                config = Deserializer.Deserialize<ApplicationTableLayoutConfig>(reader);
            }
            return new ApplicationTableLayout(this, guid, config);
        }

        protected ApplicationTableLayout CreateLayout(Guid guid)
        {
            ApplicationTableLayoutConfig config = new();
            string path = GetPathFromGuid(guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, config);
            }
            return new ApplicationTableLayout(this, guid, config);
        }

        public override void RemoveLayout(Guid guid, bool removeChildren)
        {
            if (removeChildren == true)
            {
                ApplicationTableLayoutUser tabLayout = this.UseTabLayout(guid);
                for (int i = 0; i < tabLayout.TabCount; i++)
                {
                    Guid itemAppGuid = tabLayout.GetAppGuid(i);
                    if (itemAppGuid != Guid.Empty)
                    {
                        this.ApplicationService.RemoveApplication(itemAppGuid);
                    }
                }
                tabLayout.Dispose();
            }

            FileInfo fileInfo = new FileInfo(GetPathFromGuid(guid));
            fileInfo.Delete();
            LayoutService.Registry.RemoveLayout(guid);
        }
    }

    public class ApplicationTableLayoutItemConfig
    {
        public Guid App { get; set; } = Guid.Empty;
    }
    public class ApplicationTableLayoutConfig
    {
        public List<ApplicationTableLayoutItemConfig> Tabs { get; set; } = new();
        public int ActiveTab { get; set; } = -1;

        public ApplicationTableLayoutConfig()
        {
            Tabs.Add(new ApplicationTableLayoutItemConfig());

            ActiveTab = 0;
        }
    }

    public class ApplicationTableLayout
    {
        internal ApplicationTableLayoutType LayoutType;
        public Guid Guid { get; protected set; }
        private List<ApplicationTableLayoutUser> Users = new();

        private Cooldown? SaveCooldown;

        internal ApplicationTableLayoutConfig Config;

        public ApplicationTableLayout(ApplicationTableLayoutType layoutType, Guid guid, ApplicationTableLayoutConfig config)
        {
            LayoutType = layoutType;
            Guid = guid;

            Config = config;
            if (TabCount == 0)
            {
                ActiveTab = -1;
            }
            else if (ActiveTab >= TabCount)
            {
                ActiveTab = 0;
            }

        }

        public void Dispose()
        {
            if (SaveCooldown != null)
            {

                Save();
            }

        }

        internal ApplicationTableLayoutUser CreateUser()
        {
            ApplicationTableLayoutUser user = new(this);
            Users.Add(user);
            return user;
        }
        internal void OnUserDisposed(ApplicationTableLayoutUser user)
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

        public int TabCount
        {
            get => Config.Tabs.Count;
        }

        protected void NotifyActiveTabChanged(int oldActiveTab, int newActiveTab)
        {
            foreach (var user in new List<ApplicationTableLayoutUser>(Users))
            {
                user.OnActiveTabChanged(oldActiveTab, newActiveTab);
            }
        }
        public int ActiveTab
        {
            get => Config.ActiveTab;
            set
            {

                if (value < -1)
                {
                    throw new Exception("Active tab can only be >= -1.");
                }

                if (Config.ActiveTab == value) return;
                int oldActiveTab = Config.ActiveTab;
                Config.ActiveTab = value;
                OnUpdated();
                NotifyActiveTabChanged(oldActiveTab, Config.ActiveTab);
            }
        }

        protected void NotifyTabAdded(int index)
        {
            foreach (var user in new List<ApplicationTableLayoutUser>(Users))
            {
                user.OnTabAdded(index);
            }
        }
        public void AddTab_NewApp(int index, string applicationType)
        {
            if (index < 0 || index > TabCount)
            {
                throw new Exception("Add tab index is out of range: " + index);
            }

            ApplicationUser applicationUser = LayoutType.ApplicationService.UseNewApplication(applicationType);

            Config.Tabs.Insert(index, new()
            {
                App = applicationUser.ApplicationGuid,
            });
            OnUpdated();
            try
            {
                NotifyTabAdded(index);

            }
            finally
            {
                if (ActiveTab == -1)
                {
                    ActiveTab = 0;
                }
                applicationUser.Dispose();
            }


        }


        protected void NotifyTabRemoved(int index)
        {
            foreach (var user in new List<ApplicationTableLayoutUser>(Users))
            {
                user.OnTabRemoved(index);
            }
        }
        public void RemoveTab(int index)
        {
            if (index < 0 || index >= TabCount)
            {
                throw new Exception("Remove tab index is out of range: " + index);
            }
            Guid appGuid = Config.Tabs[index].App;

            Config.Tabs.RemoveAt(index);
            OnUpdated();
            try
            {
                NotifyTabRemoved(index);
            }
            finally
            {
                if (index > 0 && index <= ActiveTab)
                {
                    ActiveTab = ActiveTab - 1;
                }
                else if (TabCount == 0)
                {
                    ActiveTab = -1;
                }

                if (appGuid != Guid.Empty)
                {
                    LayoutType.ApplicationService.RemoveApplication(appGuid);
                }

            }






        }

        public Guid GetAppGuid(int index)
        {
            if (index < 0 || index >= TabCount)
            {
                throw new Exception("Tab index is out of range: " + index);
            }
            return Config.Tabs[index].App;
        }
    }

    public class ApplicationTableLayoutUser : LayoutUser
    {
        public override string LayoutType => LoadedLayout.LayoutType.Type;

        public override Guid LayoutGuid => LoadedLayout.Guid;

        protected ApplicationTableLayout LoadedLayout;
        public ApplicationTableLayoutUser(ApplicationTableLayout loadedLayout)
        {
            LoadedLayout = loadedLayout;
        }

        public override void Dispose()
        {
            LoadedLayout.OnUserDisposed(this);
        }

        public int TabCount => LoadedLayout.TabCount;

        public int ActiveTab
        {
            get => LoadedLayout.ActiveTab;
            set => LoadedLayout.ActiveTab = value;
        }



        public Action<int, int>? ActiveTabChanged = null;

        internal void OnActiveTabChanged(int oldActiveTab, int newActiveTab)
        {
            ActiveTabChanged?.Invoke(oldActiveTab, newActiveTab);
        }

        public void AddTab_NewApp(int index, string applicationType)
        {
            LoadedLayout.AddTab_NewApp(index, applicationType);
        }
        public Action<int>? TabAdded = null;

        internal void OnTabAdded(int index)
        {
            TabAdded?.Invoke(index);
        }

        public void RemoveTab(int index)
        {
            LoadedLayout.RemoveTab(index);
        }
        public Action<int>? TabRemoved = null;

        internal void OnTabRemoved(int index)
        {
            TabRemoved?.Invoke(index);
        }

        public Guid GetAppGuid(int index)
        {
            return LoadedLayout.GetAppGuid(index);
        }
    }
}
