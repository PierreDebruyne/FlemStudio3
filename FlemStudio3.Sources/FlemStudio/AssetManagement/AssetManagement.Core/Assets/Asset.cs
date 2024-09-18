namespace FlemStudio.AssetManagement.Core.Assets
{
    public class Asset
    {


        public AssetInfo Info { get; }
        public IAssetDefinition Definition { get; }


        public Asset(AssetInfo info, IAssetDefinition definition)
        {
            Info = info;
            Definition = definition;


        }
        private IAssetContainer? _ParentDirectory;
        public IAssetContainer? ParentDirectory
        {
            get => _ParentDirectory;
            set
            {
                if (_ParentDirectory != null)
                {
                    _ParentDirectory.RemoveChild(this);
                }
                if (value != null)
                {
                    value.AddChild(this);
                }
                _ParentDirectory = value;
            }
        }
    }


}
