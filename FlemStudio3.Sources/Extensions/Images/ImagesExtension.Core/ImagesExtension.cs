using FlemStudio.ExtensionManagement.Core;
using FlemStudio.Project.Core.ExtensionManagement;

namespace ImagesExtension.Core
{

    public class ImagesExtension : ProjectExtension
    {
        public ImagesExtension(ExtensionInfo info) : base(info)
        {
            RegisterAssetType(new ImageSourceAssetType());
        }

        public override void Test()
        {
            Console.WriteLine("FlemStudio extension Images.Core");
        }



    }
}
