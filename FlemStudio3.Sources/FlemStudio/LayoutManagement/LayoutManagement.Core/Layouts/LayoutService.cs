using System.Diagnostics;

namespace FlemStudio.LayoutManagement.Core.Layouts
{


    public class LayoutService
    {
        public string FolderPath { get; }

        internal LayoutRegistry Registry;
        protected Dictionary<string, LayoutType> LayoutTypes = new();
        protected MainLayout MainLayout;



        public LayoutService(string folderPath)
        {
            FolderPath = folderPath;
            DirectoryInfo layoutFolderInfo = new DirectoryInfo(FolderPath);
            if (layoutFolderInfo.Exists == false)
            {
                layoutFolderInfo.Create();
            }

            Registry = new LayoutRegistry(FolderPath + "/" + "registry.yaml");
            MainLayout = new MainLayout(this, FolderPath + "/" + "main.yaml");
        }

        public void AddLayoutType(LayoutType layoutType)
        {
            LayoutTypes.Add(layoutType.Type, layoutType);
        }

        public LayoutType GetLayoutType(string name)
        {
            return LayoutTypes[name];
        }

        public void Update(float deltaTime)
        {
            try
            {
                Registry.Update(deltaTime);

                foreach (var type in LayoutTypes.Values)
                {
                    type.Update(deltaTime);
                }
                MainLayout?.Update(deltaTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception occured in LayoutService:");
                Debug.WriteLine(e);
            }
        }

        public void Dispose()
        {
            Registry.Dispose();

            foreach (var type in LayoutTypes.Values)
            {
                type.Dispose();
            }
            MainLayout?.Dispose();
        }

        public LayoutUser UseLayout(Guid guid)
        {
            string layoutTypeName = Registry.GetLayoutType(guid);
            if (LayoutTypes.TryGetValue(layoutTypeName, out LayoutType layoutType))
            {
                return layoutType.UseLayout(guid);
            }
            else
            {
                throw new Exception("This layout type is not implemented: " + layoutTypeName);
            }
        }

        public LayoutUser UseNewLayout(string layoutTypeName)
        {
            if (LayoutTypes.TryGetValue(layoutTypeName, out LayoutType layoutType))
            {
                return layoutType.UseNewLayout();
            }
            else
            {
                throw new Exception("This layout type is not implemented: " + layoutTypeName);
            }
        }

        public MainLayoutUser UseMainLayout()
        {
            return MainLayout.CreateUser();
        }

        public void RemoveLayout(Guid guid, bool removeChildren)
        {
            try
            {
                string layoutTypeName = Registry.GetLayoutType(guid);
                if (LayoutTypes.TryGetValue(layoutTypeName, out LayoutType layoutType))
                {
                    layoutType.RemoveLayout(guid, removeChildren);
                }
                else
                {
                    throw new Exception("This layout type is not implemented: " + layoutTypeName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        


    }

    public abstract class LayoutUser : IDisposable
    {
        public abstract string LayoutType { get; }
        public abstract Guid LayoutGuid { get; }
        public abstract void Dispose();
    }


    public abstract class LayoutType
    {
        public LayoutService LayoutService { get; protected set; }
        public string Type { get; protected set; }

        protected LayoutType(LayoutService layoutService, string type)
        {
            LayoutService = layoutService;
            Type = type;
        }
        public abstract LayoutUser UseLayout(Guid guid);
        public abstract LayoutUser UseNewLayout();

        public abstract void RemoveLayout(Guid guid, bool removeChildren);
        public abstract void Update(float deltaTime);

        public abstract void Dispose();
        protected string GetPathFromGuid(Guid guid)
        {
            return LayoutService.FolderPath + "/" + guid + "." + this.Type + ".yaml";
        }
    }

}
