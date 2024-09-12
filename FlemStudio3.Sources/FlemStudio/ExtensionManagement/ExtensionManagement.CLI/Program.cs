using ExtensionManagement.CLI;
using FlemStudio.ExtensionManagement.Core;
using System.CommandLine;

public class Program
{
    static string InstallDirectory = "C:\\Users\\Pierre\\Desktop\\FlemStudio\\FlemStudio3\\InstallFolder";
    private static void Main(string[] args)
    {

        Program program = new Program();
        program.Run(args);

    }

    protected ExtensionManager ExtensionManager;

    protected RootCommand RootCommand;
    protected CreateExtensionCommand CreateExtensionCommand;
    public Program()
    {

        ExtensionManager = new ExtensionManager(InstallDirectory + "/" + "Extensions");

        RootCommand = new RootCommand("FlemStudio extension management");

        CreateExtensionCommand = new CreateExtensionCommand(ExtensionManager);
        RootCommand.AddCommand(CreateExtensionCommand.Command);
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