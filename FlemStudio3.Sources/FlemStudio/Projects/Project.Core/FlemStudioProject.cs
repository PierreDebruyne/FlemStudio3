using FlemStudio.AssetManagement.Core;
using FlemStudio.ExtensionManagement.Core;

namespace FlemStudio.Project.Core
{
    public class FlemStudioProject
    {
        public string InstallDirectory;
        public string ProjectFilePath { get; }
        public string ProjectDirectoryPath { get; }

        public FlemStudioProjectFile ProjectFile { get; }
        public string Name => ProjectFile.Name;
        public ExtensionImporter ExtensionImporter;
        //public ProjectExtensionManager ExtensionManager { get; }
        public AssetManager AssetManager { get; }

        public FlemStudioProject(string installDirectory, string projectFilePath, List<string> contexts)
        {
            InstallDirectory = installDirectory;
            ProjectFilePath = projectFilePath;
            FileInfo projectFileInfo = new FileInfo(projectFilePath);
            if (projectFileInfo.Exists == false)
            {
                throw new FileNotFoundException("This project file does not exit: " + ProjectFilePath);
            }
            ProjectDirectoryPath = projectFileInfo.DirectoryName;
            ProjectFile = FlemStudioProjectFile.ReadFile(ProjectFilePath);
            //ExtensionManager = new ProjectExtensionManager(this);
            //ExtensionManager.LoadExtensions(ProjectFile.Extensions);
            //ExtensionManager.TestExtensions();

            ExtensionImporter = new ExtensionImporter(installDirectory + "/" + "Extensions", ProjectFile.Extensions, contexts);

            AssetManager = new AssetManager(ProjectDirectoryPath, ExtensionImporter);

            /*
            foreach (AssetType assetType in ExtensionManager.EnumerateAssetTypes())
            {
                AssetManager.RegisterAssetType(assetType);
            }
            */



        }

        public void Dispose()
        {
            AssetManager.Dispose();
        }

        public void Update(float deltaTime)
        {
            AssetManager.Update(deltaTime);
        }
    }
}
