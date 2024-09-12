using FlemStudio.ExtensionManagement.Core;
using FlemStudio.Project.Core.ExtensionManagement;

namespace TestExtension.Core
{
    public class TestExtension : ProjectExtension
    {
        public TestExtension(ExtensionInfo info) : base(info)
        {
            RegisterAssetType(new TestAssetType());
        }

        public override void Test()
        {
            Console.WriteLine("FlemStudio extension Test.Core");
        }



    }
}
