namespace FlemStudio.ExtensionManagement.Core
{
    public interface IExtensionInfo
    {
        public Guid Guid { get; }
        public string Name { get; }

        public string Version { get; }


        public string Dll_Path { get; }

        public string? Folder { get; }

        
    }
}
