using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using System.ComponentModel.Composition;

namespace FlemStudio.AssetManagement.Avalonia
{
    public interface ICreateAssetDialogType
    {
        public void Test();
        public DialogViewModel CreateDialogViewModel();
        public void OnCreateAsset(AssetInfo assetInfo, DialogViewModel dialogViewModel);
    }

    public abstract class CreateAssetDialogType<T> : ICreateAssetDialogType where T : DialogViewModel
    {

        public void OnCreateAsset(AssetInfo assetInfo, DialogViewModel dialogViewModel)
        {
            DoCreateAsset(assetInfo, (T)dialogViewModel);
        }

        public DialogViewModel CreateDialogViewModel()
        {
            return DoCreateDialogViewModel();
        }

        public abstract T DoCreateDialogViewModel();

        public abstract void DoCreateAsset(AssetInfo assetInfo, T dialogViewModel);

        public abstract void Test();
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateAssetDialogTypeAttribute : ExportAttribute, IAssetTypeIdentity
    {
        public CreateAssetDialogTypeAttribute(string name, string guid, string version) : base(typeof(ICreateAssetDialogType))
        {
            Name = name;
            Guid = guid;
            Version = version;
        }

        public string Name { get; }
        public string Guid { get; }
        public string Version { get; }

    }
}
