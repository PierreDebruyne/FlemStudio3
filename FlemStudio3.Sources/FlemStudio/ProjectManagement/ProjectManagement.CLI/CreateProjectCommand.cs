using FlemStudio.ProjectManagement.Core;
using System.CommandLine;

namespace FlemStudio.ProjectManagement.CLI
{
    public class CreateProjectCommand
    {
        protected ProjectManager ProjectManager;
        public Command Command { get; }

        public CreateProjectCommand(ProjectManager projectManager)
        {
            ProjectManager = projectManager;
            Command = new Command("create", "Create a new flem studio project.");

            var pathArgument = new Argument<string>(
                name: "path",
                description: "The folder path where the project will be created."
                );
            Command.AddArgument(pathArgument);
            var nameArgument = new Argument<string>(
                name: "name",
                description: "The name of the project."
                );
            Command.AddArgument(nameArgument);
            Command.SetHandler((folderPath, projectName) =>
            {
                try
                {
                    ProjectManager.CreateProject(folderPath, projectName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            },
            pathArgument, nameArgument);
        }
    }
}
