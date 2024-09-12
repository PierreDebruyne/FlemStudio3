using FlemStudio.Project.CLI.ExtensionManagement;
using FlemStudio.Project.Core;
using FlemStudio.Project.Core.ExtensionManagement;
using System.CommandLine;

namespace FlemStudio.Project.CLI
{
    public class ExtensionCommand
    {
        protected FlemStudioProject Project;
        protected ProjectCLIExtensionManager ProjectCLIExtensionManager;
        public Command Command { get; }

        public ExtensionCommand(FlemStudioProject project, ProjectCLIExtensionManager projectCLIExtensionManager)
        {
            Project = project;
            ProjectCLIExtensionManager = projectCLIExtensionManager;

            Command = new Command("Extensions", "Available extensions management.");

            Command listCommand = new Command("list", "List all loaded extensions.");
            listCommand.SetHandler(() =>
            {
                foreach (string extensionName in Project.ProjectFile.Extensions)
                {
                    Project.ExtensionManager.TryGetLoadedExtension(extensionName, out ProjectExtension? projectExtension);
                    projectCLIExtensionManager.TryGetLoadedExtension(extensionName + "-cli", out ProjectCLIExtension? projectCLIExtension);
                    if (projectExtension == null)
                    {
                        Console.WriteLine(extensionName + " => Not loaded.");
                    }
                    else
                    {
                        Console.WriteLine(extensionName + " => Version: " + projectExtension.Infos.Version + ", CLI: " + ((projectCLIExtension != null) ? "Loaded" : "Not loaded"));
                    }
                }
            });
            Command.AddCommand(listCommand);
        }
    }
}
