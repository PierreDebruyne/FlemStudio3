using FlemStudio.AssetManagement.CLI;
using FlemStudio.ExtensionManagement.Core;
using FlemStudio.Project.Core.ExtensionManagement;
using System.Reflection;

namespace FlemStudio.Project.CLI.ExtensionManagement
{
    public class ProjectCLIExtensionManager
    {
        protected FlemStudioProjectCLI ProjectCLI;
        protected Dictionary<string, ProjectCLIExtension> LoadedExtensions = new();
        public string ExtensionDirectoryPath;

        public ProjectCLIExtensionManager(FlemStudioProjectCLI projectCLI)
        {
            ProjectCLI = projectCLI;
            ExtensionDirectoryPath = ProjectCLI.InstallDirectory + "/" + "exts";
        }

        public void LoadExtensions(IEnumerable<ProjectExtension> loadedExtensions)
        {
            ExtensionManager extensionManager = new ExtensionManager(ProjectCLI.InstallDirectory + "/" + "Extensions");

            foreach (ProjectExtension loadedExtension in loadedExtensions)
            {
                string extensionName = loadedExtension.Infos.Name + "-cli";
                try
                {
                    ExtensionInfo extensionInfo = extensionManager.GetExtensionInfo(extensionName);
                    Assembly dll = Assembly.LoadFrom(extensionInfo.Dll_Path);
                    Type? extensionType = null;
                    Type baseExtensionType = typeof(ProjectCLIExtension);
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
                    ProjectCLIExtension extension = (ProjectCLIExtension)Activator.CreateInstance(extensionType, [extensionInfo]);
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
            foreach (ProjectCLIExtension extension in LoadedExtensions.Values)
            {
                extension.Test();
            }
        }

        public bool TryGetLoadedExtension(string extensionName, out ProjectCLIExtension? projectCLIExtension)
        {
            return LoadedExtensions.TryGetValue(extensionName, out projectCLIExtension);
        }

        public IEnumerable<ProjectCLIExtension> EnumerateLoadedExtensions()
        {
            foreach (ProjectCLIExtension extension in LoadedExtensions.Values)
            {
                yield return extension;
            }
        }

        public IEnumerable<AssetTypeCLI> EnumerateAssetTypeCLI()
        {
            foreach (ProjectCLIExtension extension in LoadedExtensions.Values)
            {
                foreach (AssetTypeCLI assetType in extension.AssetTypeRegistry.EnumerateAssetTypesCLI())
                {
                    yield return assetType;
                }
            }
        }

        /*
        public IEnumerable<AssetType> EnumerateAssetTypes()
        {
            foreach (FlemStudioExtension extension in LoadedExtensions.Values)
            {
                foreach (AssetType assetType in extension.AssetTypeRegistry.EnumerateAssetTypes())
                {
                    yield return assetType;
                }
            }
        }
        */
    }
}
