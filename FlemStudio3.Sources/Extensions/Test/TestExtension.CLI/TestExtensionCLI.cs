using FlemStudio.ExtensionManagement.Core;
using FlemStudio.Project.CLI.ExtensionManagement;

namespace TestExtension.CLI
{
    public class TestExtensionCLI : ProjectCLIExtension
    {
        public TestExtensionCLI(ExtensionInfo info) : base(info)
        {
            this.RegisterAssetTypeCLI(new TestAssetTypeCLI());
        }

        public override void Test()
        {
            Console.WriteLine("FlemStudio extension Test.CLI");
        }
    }
}
