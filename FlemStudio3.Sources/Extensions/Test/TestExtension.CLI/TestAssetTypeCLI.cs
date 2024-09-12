using FlemStudio.AssetManagement.CLI;
using TestExtension.Core;

namespace TestExtension.CLI
{
    public class TestAssetTypeCLI : AssetTypeCLI<TestAssetType>
    {
        public TestAssetTypeCLI() : base(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Test", "0.0.1")
        {
        }


    }
}
