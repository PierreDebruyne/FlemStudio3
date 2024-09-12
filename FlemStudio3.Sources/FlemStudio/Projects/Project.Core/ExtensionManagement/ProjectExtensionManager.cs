using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;
using System.Reflection;

namespace FlemStudio.Project.Core.ExtensionManagement
{
    public class ProjectExtensionManager
    {
        protected FlemStudioProject Project;
        protected Dictionary<string, ProjectExtension> LoadedExtensions = new();
        public string ExtensionDirectoryPath;

        public ProjectExtensionManager(FlemStudioProject project)
        {
            Project = project;
            ExtensionDirectoryPath = Project.InstallDirectory + "/" + "exts";
        }

        public void LoadExtensions(IList<string> extensionNames)
        {
            ExtensionManager extensionManager = new ExtensionManager(Project.InstallDirectory + "/" + "Extensions");

            foreach (string extensionName in extensionNames)
            {
                try
                {
                    ExtensionInfo extensionInfo = extensionManager.GetExtensionInfo(extensionName);
                    Assembly dll = Assembly.LoadFrom(extensionInfo.Dll_Path);
                    Type? extensionType = null;
                    Type baseExtensionType = typeof(ProjectExtension);
                    foreach (Type type in dll.GetExportedTypes())
                    {
                        if (type.IsAssignableTo(baseExtensionType))
                        {
                            if (extensionType == null)
                            {
                                extensionType = type;
                            }
                            else
                            {
                                throw new Exception("Multiple " + baseExtensionType.Name + " class detected.");
                            }
                        }
                    }
                    if (extensionType == null)
                    {
                        throw new Exception("FlemStudioExtension class not found.");
                    }
                    ProjectExtension extension = (ProjectExtension)Activator.CreateInstance(extensionType, [extensionInfo]);
                    LoadedExtensions.Add(extensionName, extension);
                    Console.WriteLine("Extension loadded: " + extension.Infos.Name + ", version: " + extensionInfo.Version);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Impossible to load extension: " + extensionName);
                    Console.WriteLine(ex.ToString());
                }

            }
        }

        public void TestExtensions()
        {
            foreach (ProjectExtension extension in LoadedExtensions.Values)
            {
                extension.Test();
            }
        }

        public bool TryGetLoadedExtension(string extensionName, out ProjectExtension? projectExtension)
        {
            return LoadedExtensions.TryGetValue(extensionName, out projectExtension);
        }

        public IEnumerable<ProjectExtension> EnumerateLoadedExtensions()
        {
            foreach (ProjectExtension extension in LoadedExtensions.Values)
            {
                yield return extension;
            }
        }

        public IEnumerable<AssetType> EnumerateAssetTypes()
        {
            foreach (ProjectExtension extension in LoadedExtensions.Values)
            {
                foreach (AssetType assetType in extension.AssetTypeRegistry.EnumerateAssetTypes())
                {
                    yield return assetType;
                }
            }
        }
    }
}
