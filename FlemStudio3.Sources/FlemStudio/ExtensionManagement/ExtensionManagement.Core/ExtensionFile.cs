namespace FlemStudio.ExtensionManagement.Core
{
    public class ExtensionFile : IExtensionInfo
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Dll_Path { get; set; }
        public string? Folder { get; set; }
    }
}
