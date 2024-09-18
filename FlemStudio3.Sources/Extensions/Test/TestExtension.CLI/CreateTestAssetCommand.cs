using FlemStudio.AssetManagement.CLI.Assets.Create;
using FlemStudio.AssetManagement.Core.Assets;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel.Composition;
using TestExtension.Core;

namespace TestExtension.CLI
{
    [CreateAssetCommandType(name: "Test", guid: "00000000-0000-0000-0000-000000000001", version: "0.0.1")]
    public class CreateTestAssetCommand : ICreateAssetCommandType
    {

        [Import(typeof(TestAssetType), RequiredCreationPolicy = CreationPolicy.Shared)]
        public TestAssetType TestAssetType { get; set; }

        protected Option<string> HelloOption;

        public void Test()
        {
            if (TestAssetType != null)
            {
                Console.WriteLine("YO, I'am the Create Test asset command.");
                TestAssetType.Test();
            }
            else
            {
                Console.WriteLine("BUG, I'am the Create Test asset command.");
            }
        }

        public void ModifyCommand(Command command)
        {
            HelloOption = new Option<string>(
               name: "--hello",
               description: "The hello field of the config file generated.",
                getDefaultValue: () => "bonjour"
               );
            command.AddOption(HelloOption);
        }

        public void OnCreateAsset(AssetInfo assetInfo, InvocationContext context)
        {
            string hello = context.ParseResult.GetValueForOption(HelloOption);
            TestAssetType.OnCreateAsset(assetInfo, hello);
        }


    }
}
