using FlemStudio.ExtensionManagement.Core;
using System.CommandLine;

namespace FlemStudio.ExtensionManagement.CLI
{
    public class CreateExtensionCommand
    {
        protected ExtensionManager ExtensionManager;
        public Command Command { get; }

        public CreateExtensionCommand(ExtensionManager extensionManager)
        {
            ExtensionManager = extensionManager;
            Command = new Command("create", "Create a new extension.");
            var nameArgument = new Argument<string>(
                name: "name",
                description: "The name of the extension you want to create."
                );
            Command.AddArgument(nameArgument);
            var contextArgument = new Argument<string>(
                name: "context",
                description: "The context where the extension need to be loaded."
                );
            Command.AddArgument(contextArgument);
            var ddlPathArgument = new Argument<string>(
                name: "dll_path",
                description: "The path of the ddl file for the extension."
                );
            Command.AddArgument(ddlPathArgument);
            Command.SetHandler((name, context, dll_path) =>
            {
                try
                {
                    ExtensionManager.CreateExtension(name, context, dll_path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            },
            nameArgument, contextArgument, ddlPathArgument);
        }
    }
}
