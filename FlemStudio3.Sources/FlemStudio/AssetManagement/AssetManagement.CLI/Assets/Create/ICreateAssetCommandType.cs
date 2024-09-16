using FlemStudio.AssetManagement.Core;
using FlemStudio.AssetManagement.Core.Assets;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlemStudio.AssetManagement.CLI.Assets.Create
{
    public interface ICreateAssetCommandType
    {
        public abstract void Test();
        public void ModifyCommand(Command command);
        public void OnCreateAsset(AssetInfo assetInfo, InvocationContext context);
        

    }


    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateAssetCommandTypeAttribute : ExportAttribute, IAssetTypeIdentity
    {
        public CreateAssetCommandTypeAttribute(string name, string guid, string version) : base(typeof(ICreateAssetCommandType))
        {
            Name = name;
            Guid = guid;
            Version = version;
            Context = "cli";
        }

        public string Name { get; }
        public string Guid { get; }
        public string Version { get; }
        public string Context { get; }

    }
}
