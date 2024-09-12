using FlemStudio.AssetManagement.Core;
using System.CommandLine;

namespace FlemStudio.AssetManagement.CLI
{
    public abstract class AssetTypeCLI
    {
        internal AssetType? AssetTypeBase;
        public Guid Guid { get; }
        public string Name { get; }
        public string Version { get; }

        protected AssetTypeCLI(Guid guid, string name, string version)
        {
            Guid = guid;
            Name = name;
            Version = version;


        }

    }
    public abstract class AssetTypeCLI<T> : AssetTypeCLI where T : AssetType
    {
        public T? AssetType => (T?)AssetTypeBase;


        protected AssetTypeCLI(Guid guid, string name, string version) : base(guid, name, version)
        {


        }

    }
}
