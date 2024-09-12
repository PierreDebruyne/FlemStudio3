

using System;
using System.Collections.Concurrent;

namespace FlemStudio.LayoutManagement.Core.Applications
{
    public class ApplicationService
    {
        public string FolderPath { get; }

        protected ApplicationRegistry Registry;


        protected Dictionary<string, ApplicationType> ApplicationTypesByName = new();
        protected Dictionary<Type, ApplicationType> ApplicationTypesByType = new();
        protected Dictionary<Type, ApplicationType> ApplicationTypesByUserType = new();

        protected ConcurrentDictionary<Guid, LoadedApplication> LoadedApplicationsByGuid = new();
        protected ConcurrentDictionary<string, LoadedApplication> LoadedApplicationsByTag = new();

        public ApplicationService(string folderPath)
        {
            FolderPath = folderPath;
            DirectoryInfo appFolderInfo = new DirectoryInfo(FolderPath);
            if (appFolderInfo.Exists == false)
            {
                appFolderInfo.Create();
            }

            Registry = new ApplicationRegistry(FolderPath + "/" + "registry.yaml");
        }

        public void AddApplicationType<TApplicationType>(TApplicationType applicationType) where TApplicationType : ApplicationType
        {
            ApplicationTypesByName.Add(applicationType.Name, applicationType);
            ApplicationTypesByType.Add(typeof(TApplicationType), applicationType);
            ApplicationTypesByUserType.Add(applicationType.UserType, applicationType);
        }
        public TApplicationType GetApplicationType<TApplicationType>() where TApplicationType : ApplicationType
        {
            return (TApplicationType)ApplicationTypesByType[typeof(TApplicationType)];
        }
        public ApplicationType GetApplicationType(string name)
        {
            return ApplicationTypesByName[name];
        }




        public ApplicationUser UseApplication(Guid guid)
        {
            if (LoadedApplicationsByGuid.TryGetValue(guid, out LoadedApplication loadedApplication))
            {
                return loadedApplication.CreateUser();
            }

            ApplicationRegistryItem entry = Registry.GetApplicationEntry(guid);

            if (ApplicationTypesByName.TryGetValue(entry.Type, out ApplicationType applicationType))
            {
                Application application = applicationType.LoadApplication(GetApplicationPath(guid, applicationType));
                loadedApplication = new LoadedApplication(this, guid, entry.Tag, applicationType, application);
                LoadedApplicationsByGuid.TryAdd(loadedApplication.Guid, loadedApplication);
                if (loadedApplication.Tag != null)
                {
                    LoadedApplicationsByTag.TryAdd(loadedApplication.Tag, loadedApplication);
                }
                return loadedApplication.CreateUser();
            }
            else
            {
                throw new Exception("This application type is not implemented: " + entry.Type);
            }
        }

        public TApplicationUser UseApplication<TApplicationUser>(Guid guid) where TApplicationUser : ApplicationUser
        {
            ApplicationUser user = UseApplication(guid);
            try
            {
                return (TApplicationUser)user;
            } catch (Exception ex)
            {
                user.Dispose();
                throw new Exception("Wrong cast", ex);
            }
        }

        

        public ApplicationUser UseNewApplication(string applicationTypeName, string? tag = null)
        {
            if (tag != null && Registry.ContainTag(tag))
            {
                throw new Exception("This application tag is already used: " + tag);
            }
            if (ApplicationTypesByName.TryGetValue(applicationTypeName, out ApplicationType applicationType))
            {
                
                Guid guid = Registry.NewGuid();
                Application application = applicationType.CreateApplication(GetApplicationPath(guid, applicationType));
                Registry.AddApplication(guid, applicationTypeName, tag);
                LoadedApplication loadedApplication = new LoadedApplication(this, guid, tag, applicationType, application);
                LoadedApplicationsByGuid.TryAdd(loadedApplication.Guid, loadedApplication);
                if (loadedApplication.Tag != null)
                {
                    LoadedApplicationsByTag.TryAdd(loadedApplication.Tag, loadedApplication);
                }
                return loadedApplication.CreateUser();
            }
            else
            {
                throw new Exception("This application type is not implemented: " + applicationTypeName);
            }
        }

        public TApplicationUser UseNewApplication<TApplicationUser>(string? tag = null) where TApplicationUser : ApplicationUser
        {
            if (ApplicationTypesByUserType.TryGetValue(typeof(TApplicationUser), out ApplicationType applicationType))
            {
                return (TApplicationUser)UseNewApplication(applicationType.Name, tag);
            } else
            {
                throw new Exception("This application user type is not implemented: " + typeof(TApplicationUser));
            }
        }

        internal void OnApplicationUnused(LoadedApplication loadedApplication)
        {
            LoadedApplicationsByGuid.TryRemove(new KeyValuePair<Guid, LoadedApplication>(loadedApplication.Guid, loadedApplication));
            if (loadedApplication.Tag != null)
            {
                LoadedApplicationsByTag.TryRemove(new KeyValuePair<string, LoadedApplication>(loadedApplication.Tag, loadedApplication));
            }
            loadedApplication.Dispose();
            if (loadedApplication.Tag != null)
            {
                RemoveApplication(loadedApplication.Guid);
            }
        }

        protected string GetApplicationPath(Guid guid, ApplicationType applicationType)
        {
            return FolderPath + "/" + guid + "." + applicationType.FileExtension;
        }

        internal void SaveApplication(LoadedApplication loadedApplication)
        {
            loadedApplication.ApplicationType.SaveApplication(loadedApplication.Application, GetApplicationPath(loadedApplication.Guid, loadedApplication.ApplicationType));
        }

        public void RemoveApplication(Guid guid)
        {
            ApplicationRegistryItem entry = Registry.GetApplicationEntry(guid);
            if (ApplicationTypesByName.TryGetValue(entry.Type, out ApplicationType applicationType))
            {
                FileInfo fileInfo = new FileInfo(GetApplicationPath(guid, applicationType));
                fileInfo.Delete();
                Registry.RemoveApplication(guid);
            }
            else
            {
                throw new Exception("This application type is not implemented: " + entry.Type);
            }

            
        }

        public void Update(float deltaTime)
        {
            Registry.Update(deltaTime);
            foreach (LoadedApplication loadedApplication in LoadedApplicationsByGuid.Values)
            {
                loadedApplication.Update(deltaTime);
            }
        }

        public void Dispose()
        {
            Registry.Dispose();
            foreach (LoadedApplication loadedApplication in LoadedApplicationsByGuid.Values)
            {
                loadedApplication.Dispose();
            }
        }
    }
}
