using FlemStudio.ExtensionManagement.Core;
using System.CommandLine;

namespace FlemStudio.ExtensionManagement.CLI
{
    public class UpdateExtensionsCommand
    {
        protected ExtensionManager ExtensionManager;
        public Command Command { get; }

        public UpdateExtensionsCommand(ExtensionManager extensionManager)
        {
            ExtensionManager = extensionManager;
            Command = new Command("update", "Update an extension.");
            var nameArgument = new Argument<string>(
                name: "name",
                description: "The name of the extension you want to create."
                );
            Command.AddArgument(nameArgument);

            Command.SetHandler((name) =>
            {
                try
                {
                    if (name == "all")
                    {
                        ExtensionManager.UpdateLocalExtensions();
                    }
                    else
                    {
                        ExtensionManager.UpdateLocalExtension(name);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            },
            nameArgument);
        }
    }
}
