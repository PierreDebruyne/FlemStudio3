using FlemStudio.AssetManagement.CLI;
using FlemStudio.Project.Core;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace FlemStudio.Project.CLI
{
    public class FlemStudioProjectCLI
    {
        public string InstallDirectory { get; }
        protected FlemStudioProject Project;

        protected bool Closing = false;
        protected Task? UpdateTask;

        //protected ProjectCLIExtensionManager CLIExtensionManager;

        //protected RootCommand RootCommand;

        protected AssetTypeCommand AssetTypeCommand;
        protected AssetManagerCLI AssetManagerCLI;

        public FlemStudioProjectCLI(string installDirectory, FlemStudioProject project)
        {
            InstallDirectory = installDirectory;
            Project = project;





            AssetTypeCommand = new AssetTypeCommand(Project.AssetManager);

            AssetManagerCLI = new AssetManagerCLI(Project.AssetManager, project.ExtensionImporter);



            UpdateTask = new Task(OnUpdate);
            UpdateTask.Start();
            Console.WriteLine("Update task launched.");

        }

        public bool RunCommand(string commandLine)
        {
            string[] splitted = CommandLineStringSplitter.Instance.Split(commandLine).ToArray();
            string command = splitted[0];
            string[] args = (splitted.Length > 1) ? splitted.Skip(1).ToArray() : [];

            switch (command)
            {

                case "exit":
                    return false;
                    break;
                case "help":
                    HelpCommand();
                    break;
                case "AssetTypes":
                    AssetTypeCommand.Command.Invoke(args);
                    break;
                case "Asset":
                    AssetManagerCLI.AssetsCLI.Command.Invoke(args);
                    break;
                case "AssetDirectory":
                    AssetManagerCLI.AssetDirectoryCLI.Command.Invoke(args);
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;

            }
            return true;
        }

        protected void HelpCommand()
        {
            Console.WriteLine("=== AssetTypes ===");
            AssetTypeCommand.Command.Invoke("--help");
            Console.WriteLine("=== AssetDirectory ===");
            AssetManagerCLI.AssetDirectoryCLI.Command.Invoke("--help");
            Console.WriteLine("=== Asset ===");
            AssetManagerCLI.AssetsCLI.Command.Invoke("--help");

        }

        private void OnUpdate()
        {
            DateTime lastUpdate = DateTime.Now;
            while (Closing == false)
            {
                DateTime now = DateTime.Now;
                float deltaTime = (float)(now - lastUpdate).TotalSeconds;
                Project.Update(deltaTime);


                lastUpdate = now;
                Thread.Sleep(200);
            }
        }

        public void Dipose()
        {
            Closing = true;
            if (UpdateTask != null)
            {
                Task.WaitAll(new[] { UpdateTask });
                UpdateTask.Dispose();
                Console.WriteLine("Update task stopped.");
            }
        }
    }
}
