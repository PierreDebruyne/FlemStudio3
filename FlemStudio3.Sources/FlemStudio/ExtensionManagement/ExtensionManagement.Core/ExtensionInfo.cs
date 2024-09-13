namespace FlemStudio.ExtensionManagement.Core
{
    public interface IExtensionInfo
    {
        public Guid Guid { get; }
        public string Name { get; }

        public string Version { get; }
        public List<string> Dll_Paths { get; }

    }
}
