namespace FlemStudio.ProjectManagement.Core
{
    public class ProjectManager
    {
        public void CreateProject(string folderPath, string name)
        {
            if (Directory.Exists(folderPath) == false)
            {
                throw new Exception("This folder does not exist: " + folderPath);
            }
            string projectFolderPath = folderPath + "/" + name;
            if (Directory.Exists(projectFolderPath))
            {
                throw new Exception("This folder already exist: " + projectFolderPath);
            }
            FlemStudioProjectFile file = new FlemStudioProjectFile()
            {
                Name = name,
                Version = "0.0.1",
            };
            Directory.CreateDirectory(projectFolderPath);
            FlemStudioProjectFile.WriteFile(projectFolderPath + "/" + name + ".flproj", file);
        }
    }
}
