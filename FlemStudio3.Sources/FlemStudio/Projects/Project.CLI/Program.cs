using FlemStudio.Project.CLI;
using FlemStudio.Project.Core;

public class Program
{
    static string InstallDirectory = "C:\\Users\\Pierre\\Desktop\\FlemStudio\\FlemStudio3\\InstallFolder";
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("You need to specify a project file to open.");
            return;
        }

        Program program = new Program(args[0]);
        program.Run();
    }


    public string WorkingDirectory { get; }
    protected FlemStudioProject Project;
    protected FlemStudioProjectCLI ProjectCLI;

    public Program(string projectFilePath)
    {

        Project = new FlemStudioProject(InstallDirectory, projectFilePath, ["core", "cli"]);
        ProjectCLI = new FlemStudioProjectCLI(InstallDirectory, Project);
    }

    public void Dispose()
    {
        ProjectCLI.Dipose();
        Project.Dispose();

    }

    public void Run()
    {


        string? command;
        bool mustStop = false;

        do
        {
            Console.Write(Project.Name + " -> ");
            command = Console.ReadLine();

            if (command != null && command.Length > 0)
            {
                if (ProjectCLI.RunCommand(command) == false)
                {
                    mustStop = true;
                }
                else
                {
                    Console.WriteLine();
                }

            }

        } while (mustStop == false);



        this.Dispose();
    }
}