
using FlemStudio.LayoutManagement.Core.Layouts;
using System.Collections.Concurrent;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Editors
{
    public class EditorTableLayoutItemConfig
    {
        public Guid AssetGuid { get; set; } = Guid.Empty;
    }
    public class EditorTableLayoutConfig
    {
        public List<EditorTableLayoutItemConfig> Tabs { get; set; } = new();
        public int ActiveTab { get; set; } = -1;

        public EditorTableLayoutConfig()
        {

        }
    }

    public class EditorTableLayoutType : LayoutType
    {

        protected ConcurrentDictionary<Guid, LoadedEditorTableLayout> LoadedLayouts = new();
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;
        public EditorTableLayoutType(LayoutService layoutService) : base(layoutService, "EditorTab")
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

        internal void OnLayoutUnused(LoadedEditorTableLayout loadedLayout)
        {
            LoadedLayouts.TryRemove(new KeyValuePair<Guid, LoadedEditorTableLayout>(loadedLayout.Guid, loadedLayout));
            loadedLayout.Dispose();
        }

        internal void SaveLayout(LoadedEditorTableLayout layout)
        {
            string path = GetPathFromGuid(layout.Guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, layout.Config);
            }
            Debug.WriteLine("Editor tab layout saved: " + path);
        }


        public override LayoutUser UseLayout(Guid guid)
        {
            return UseEditorTabLayout(guid);
        }

        public EditorTableLayoutUser UseEditorTabLayout(Guid guid)
        {
            LoadedEditorTableLayout loadedLayout;
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
            return UseNewEditorTabLayout();
        }

        public EditorTableLayoutUser UseNewEditorTabLayout()
        {
            Guid guid = LayoutService.Registry.AddLayout(Type);
            LoadedEditorTableLayout loadedLayout = CreateLayout(guid);
            LoadedLayouts.TryAdd(guid, loadedLayout);
            return loadedLayout.CreateUser();
        }
        protected LoadedEditorTableLayout LoadLayout(Guid guid)
        {
            string path = GetPathFromGuid(guid);
            EditorTableLayoutConfig config;
            using (TextReader reader = File.OpenText(path))
            {
                config = Deserializer.Deserialize<EditorTableLayoutConfig>(reader);
            }
            return new LoadedEditorTableLayout(this, guid, config);
        }

        protected LoadedEditorTableLayout CreateLayout(Guid guid)
        {
            EditorTableLayoutConfig config = new();
            string path = GetPathFromGuid(guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, config);
            }
            return new LoadedEditorTableLayout(this, guid, config);
        }

        public override void RemoveLayout(Guid guid, bool removeChildren)
        {
            if (removeChildren == true)
            {
                EditorTableLayoutUser tabLayout = UseEditorTabLayout(guid);
                for (int i = 0; i < tabLayout.TabCount; i++)
                {
                    Guid itemAssetGuid = tabLayout.GetAssetGuid(i);
                    if (itemAssetGuid != Guid.Empty)
                    {
                        //this.ApplicationService.RemoveApplication(itemAppGuid);
                    }
                }
                tabLayout.Dispose();
            }

            FileInfo fileInfo = new FileInfo(GetPathFromGuid(guid));
            fileInfo.Delete();
            LayoutService.Registry.RemoveLayout(guid);
        }
    }
    public class LoadedEditorTableLayout
    {
        internal EditorTableLayoutType LayoutType;
        public Guid Guid { get; protected set; }
        private List<EditorTableLayoutUser> Users = new();

        private Cooldown? SaveCooldown;

        internal EditorTableLayoutConfig Config;

        public LoadedEditorTableLayout(EditorTableLayoutType layoutType, Guid guid, EditorTableLayoutConfig config)
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

        internal EditorTableLayoutUser CreateUser()
        {
            EditorTableLayoutUser user = new(this);
            Users.Add(user);
            return user;
        }
        internal void OnUserDisposed(EditorTableLayoutUser user)
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
            foreach (var user in new List<EditorTableLayoutUser>(Users))
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
            foreach (var user in new List<EditorTableLayoutUser>(Users))
            {
                user.OnTabAdded(index);
            }
        }
        public void AddTab_NewEditor(int index, Guid assetGuid)
        {
            if (index < 0 || index > TabCount)
            {
                throw new Exception("Add tab index is out of range: " + index);
            }

            //ApplicationUser applicationUser = LayoutType.ApplicationService.UseNewApplication(applicationType);

            bool alreadyOpen = false;
            int position = 0;
            foreach (var tab in Config.Tabs)
            {
                if (tab.AssetGuid == assetGuid)
                {
                    alreadyOpen = true;
                    break;
                }
                position++;
            }
            if (alreadyOpen == false)
            {
                Config.Tabs.Insert(index, new()
                {
                    AssetGuid = assetGuid,
                });
                OnUpdated();
                try
                {
                    NotifyTabAdded(index);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }



            ActiveTab = position;


        }


        protected void NotifyTabRemoved(int index)
        {
            foreach (var user in new List<EditorTableLayoutUser>(Users))
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
            Guid assetGuid = Config.Tabs[index].AssetGuid;

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

                if (assetGuid != Guid.Empty)
                {
                    //LayoutType.ApplicationService.RemoveApplication(appGuid);
                }

            }






        }

        public Guid GetAssetGuid(int index)
        {
            if (index < 0 || index >= TabCount)
            {
                throw new Exception("Tab index is out of range: " + index);
            }
            return Config.Tabs[index].AssetGuid;
        }
    }

    public class EditorTableLayoutUser : LayoutUser
    {
        public override string LayoutType => LoadedLayout.LayoutType.Type;

        public override Guid LayoutGuid => LoadedLayout.Guid;

        protected LoadedEditorTableLayout LoadedLayout;
        public EditorTableLayoutUser(LoadedEditorTableLayout loadedLayout)
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

        public void AddTab_NewEditor(int index, Guid assetGuid)
        {
            LoadedLayout.AddTab_NewEditor(index, assetGuid);
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

        public Guid GetAssetGuid(int index)
        {
            return LoadedLayout.GetAssetGuid(index);
        }
    }
}
