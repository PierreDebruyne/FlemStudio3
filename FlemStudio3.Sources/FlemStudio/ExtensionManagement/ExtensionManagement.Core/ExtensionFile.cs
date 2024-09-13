namespace FlemStudio.ExtensionManagement.Core
{
    public class ExtensionFile : IExtensionInfo
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<string> Dll_Paths { get; set; }
    }
}
