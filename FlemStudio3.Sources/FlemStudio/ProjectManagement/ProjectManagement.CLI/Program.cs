using FlemStudio.ProjectManagement.CLI;
using FlemStudio.ProjectManagement.Core;
using System.CommandLine;

public class Program
{
    static string InstallDirectory = "C:\\Users\\Pierre\\Desktop\\FlemStudio\\FlemStudio3\\InstallFolder";
    private static void Main(string[] args)
    {

        Program program = new Program();
        program.Run(args);

    }

    protected ProjectManager ProjectManager;

    protected RootCommand RootCommand;
    protected CreateProjectCommand CreateProjectCommand;
    public Program()
    {

        ProjectManager = new ProjectManager();

        RootCommand = new RootCommand("FlemStudio project management");

        CreateProjectCommand = new CreateProjectCommand(ProjectManager);
        RootCommand.AddCommand(CreateProjectCommand.Command);
    }

    public void Run(string[] args)
    {
        try
        {
            RootCommand.Invoke(args);
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }
}