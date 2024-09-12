using System.Collections.Concurrent;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Layouts
{

    public class RowLayoutType : LayoutType
    {
        protected ConcurrentDictionary<Guid, RowLayout> LoadedLayouts = new();
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;
        public RowLayoutType(LayoutService layoutService) : base(layoutService, "Row")
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

        internal void OnLayoutUnused(RowLayout loadedLayout)
        {
            LoadedLayouts.TryRemove(new KeyValuePair<Guid, RowLayout>(loadedLayout.Guid, loadedLayout));
            loadedLayout.Dispose();
        }

        
        internal void SaveLayout(RowLayout layout)
        {
            string path = GetPathFromGuid(layout.Guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, layout.Config);
            }
            Debug.WriteLine("Row layout saved: " + path);
        }


        public override LayoutUser UseLayout(Guid guid)
        {
            return UseRowLayout(guid);
        }

        public RowLayoutUser UseRowLayout(Guid guid)
        {
            RowLayout loadedLayout;
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
            return UseNewRowLayout();
        }

        public RowLayoutUser UseNewRowLayout()
        {
            Guid guid = LayoutService.Registry.AddLayout(this.Type);
            RowLayout loadedLayout = CreateLayout(guid);
            LoadedLayouts.TryAdd(guid, loadedLayout);
            return loadedLayout.CreateUser();
        }
        protected RowLayout LoadLayout(Guid guid)
        {
            string path = GetPathFromGuid(guid);
            RowLayoutConfig config;
            using (TextReader reader = File.OpenText(path))
            {
                config = Deserializer.Deserialize<RowLayoutConfig>(reader);
            }
            return new RowLayout(this, guid, config);
        }

        protected RowLayout CreateLayout(Guid guid)
        {
            RowLayoutConfig config = new();
            string path = GetPathFromGuid(guid);
            using (TextWriter writer = File.CreateText(path))
            {
                Serializer.Serialize(writer, config);
            }
            return new RowLayout(this, guid, config);
        }

        public override void RemoveLayout(Guid guid, bool removeChildren)
        {
            if (removeChildren == true)
            {
                RowLayoutUser rowLayout = this.UseRowLayout(guid);
                for (int i = 0; i < rowLayout.ItemCount; i++)
                {
                    Guid itemLayoutGuid = rowLayout.GetLayoutGuid(i);
                    if (itemLayoutGuid != Guid.Empty)
                    {
                        this.LayoutService.RemoveLayout(itemLayoutGuid, removeChildren);
                    }
                }
                rowLayout.Dispose();
            }


            FileInfo fileInfo = new FileInfo(GetPathFromGuid(guid));
            fileInfo.Delete();
            LayoutService.Registry.RemoveLayout(guid);
        }
    }
    public class RowLayoutItemConfig
    {
        public Guid Layout { get; set; } = Guid.Empty;
        public float Size { get; set; }

        public RowLayoutItemConfig()
        {
        }

        public RowLayoutItemConfig(float size, Guid layoutGuid)
        {
            Size = size;
            Layout = layoutGuid;
        }
    }
    public class RowLayoutConfig
    {
        public List<RowLayoutItemConfig> Items { get; set; } = new();

        public RowLayoutConfig()
        {
            Items.Add(new RowLayoutItemConfig(0.5f, Guid.Empty));
            Items.Add(new RowLayoutItemConfig(0.5f, Guid.Empty));

        }
    }

    public class RowLayout
    {
        internal RowLayoutType LayoutType;
        public Guid Guid { get; protected set; }
        private List<RowLayoutUser> Users = new();

        private Cooldown? SaveCooldown;

        internal RowLayoutConfig Config;

        public RowLayout(RowLayoutType layoutType, Guid guid, RowLayoutConfig config)
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

        internal RowLayoutUser CreateUser()
        {
            RowLayoutUser user = new(this);
            Users.Add(user);
            return user;
        }
        internal void OnUserDisposed(RowLayoutUser user)
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
        public int ItemCount
        {
            get => Config.Items.Count;
        }

        protected void NotifyItemAdded(int index)
        {
            foreach (var user in new List<RowLayoutUser>(Users))
            {
                user.OnItemAdded(index);
            }
        }
        public void AddItem(int index, Guid layoutGuid)
        {
            if (index < 0 || index > ItemCount)
            {
                throw new Exception("Add item index is out of range: " + index);
            }
            float size = 1f;
            if (ItemCount > 0)
            {
                if (index < ItemCount)
                {
                    size = Config.Items[index].Size / 2.0f;
                    Config.Items[index].Size /= 2.0f;
                }
                else
                {
                    size = Config.Items[index - 1].Size / 2.0f;
                    Config.Items[index - 1].Size /= 2.0f;
                }
            }
            Config.Items.Insert(index, new(size, layoutGuid));
            OnUpdated();
            NotifyItemAdded(index);
        }
        protected void NotifyItemRemoved(int index, Guid itemGuid)
        {
            foreach (var user in new List<RowLayoutUser>(Users))
            {
                user.OnItemRemoved(index, itemGuid);
            }
        }
        public void RemoveItem(int index)
        {
            if (index < 0 || index >= ItemCount)
            {
                throw new Exception("Remove item index is out of range: " + index);
            }
            Guid itemLayoutGuid = Config.Items[index].Layout;
            float size = Config.Items[index].Size;
            if (index == 0)
            {
                Config.Items[index + 1].Size += size;
            }
            else if (index == ItemCount - 1)
            {
                Config.Items[index - 1].Size += size;
            }
            else
            {
                Config.Items[index - 1].Size += size / 2.0f;
                Config.Items[index + 1].Size += size / 2.0f;
            }
            Config.Items.RemoveAt(index);
            OnUpdated();
            NotifyItemRemoved(index, itemLayoutGuid);

        }

        public Guid GetLayoutGuid(int index)
        {
            if (index < 0 || index >= ItemCount)
            {
                throw new Exception("Item index is out of range: " + index);
            }
            return Config.Items[index].Layout;
        }

        protected void NotifyItemLayoutGuidChanged(int index, Guid oldLayoutGuid, Guid newLayoutGuid)
        {
            foreach (var user in new List<RowLayoutUser>(Users))
            {
                user.OnItemLayoutGuidChanged(index, oldLayoutGuid, newLayoutGuid);
            }
        }
        public void SetLayoutGuid(int index, Guid guid)
        {
            if (index < 0 || index >= ItemCount)
            {
                throw new Exception("Item index is out of range: " + index);
            }
            if (Config.Items[index].Layout == guid) return;
            Guid oldGuid = Config.Items[index].Layout;
            Config.Items[index].Layout = guid;
            OnUpdated();
            NotifyItemLayoutGuidChanged(index, oldGuid, guid);


        }

        public float GetSize(int index)
        {
            if (index < 0 || index >= ItemCount)
            {
                throw new Exception("Item index is out of range: " + index);
            }
            return Config.Items[index].Size;
        }
        public void ResizeItem(int index, float size)
        {
            if (index < 0 || index >= ItemCount)
            {
                throw new Exception("Item index is out of range: " + index);
            }
            Config.Items[index].Size = size;
            OnUpdated();
        }
    }

    public class RowLayoutUser : LayoutUser
    {

        public override string LayoutType => LoadedLayout.LayoutType.Type;

        public override Guid LayoutGuid => LoadedLayout.Guid;

        protected RowLayout LoadedLayout;

        public RowLayoutUser(RowLayout loadedLayout)
        {
            LoadedLayout = loadedLayout;
        }

        public override void Dispose()
        {
            LoadedLayout.OnUserDisposed(this);
        }

        public int ItemCount => LoadedLayout.ItemCount;



        public void AddItem(int index, Guid layoutGuid)
        {
            LoadedLayout.AddItem(index, layoutGuid);
        }

        public Action<int>? ItemAdded = null;
        internal void OnItemAdded(int index)
        {
            ItemAdded?.Invoke(index);
        }

        public void RemoveItem(int index)
        {
            LoadedLayout.RemoveItem(index);
        }

        public Action<int, Guid>? ItemRemoved = null;
        internal void OnItemRemoved(int index, Guid itemGuid)
        {
            ItemRemoved?.Invoke(index, itemGuid);
        }

        public Guid GetLayoutGuid(int index)
        {
            return LoadedLayout.GetLayoutGuid(index);
        }

        public Action<int, Guid, Guid>? ItemLayoutGuidChanged;
        internal void OnItemLayoutGuidChanged(int index, Guid oldLayoutGuid, Guid newLayoutGuid)
        {
            ItemLayoutGuidChanged?.Invoke(index, oldLayoutGuid, newLayoutGuid);
        }
        public void SetLayoutGuid(int index, Guid guid)
        {
            LoadedLayout.SetLayoutGuid(index, guid);
        }

        public float GetSize(int index)
        {
            return LoadedLayout.GetSize(index);
        }

        public void ResizeItem(int index, float size)
        {
            LoadedLayout.ResizeItem(index, size);
        }
    }
}
