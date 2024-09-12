namespace FlemStudio.ExtensionManagement.Core
{
    public class ExtensionInfo
    {
        protected ExtensionManager ExtensionManager;
        public Guid Guid { get; }
        public string Name { get; }

        public string Version { get; }


        public string Dll_Path { get; }

        public ExtensionInfo(ExtensionManager extensionManager, Guid guid, string name, string version, string dll_Path)
        {
            ExtensionManager = extensionManager;
            Guid = guid;
            Name = name;
            Version = version;
            Dll_Path = dll_Path;
        }
    }
}
