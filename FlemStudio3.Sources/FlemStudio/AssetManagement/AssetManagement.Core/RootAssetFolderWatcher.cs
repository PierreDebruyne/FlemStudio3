using FlemStudio.AssetManagement.Core.RootAssetDirectories;

namespace FlemStudio.AssetManagement.Core
{
    public class RootAssetFolderWatcher
    {
        protected RootAssetDirectory RootAssetDirectory;
        protected FileSystemWatcher FileSystemWatcher;

        static float UpdateTime = 0.5f;
        protected float TimeElapsed = 0f;

        protected List<string> FilesCreated = new();
        public Action<RootAssetDirectory, string>? OnFileCreated;

        protected List<string> FilesDeleted = new();
        public Action<RootAssetDirectory, string>? OnFileDeleted;

        public RootAssetFolderWatcher(RootAssetDirectory rootAssetFolder)
        {
            RootAssetDirectory = rootAssetFolder;

            FileSystemWatcher = new FileSystemWatcher(RootAssetDirectory.Info.FullPath);

            FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                //NotifyFilters.Attributes
            //                     | NotifyFilters.CreationTime
            //                     | NotifyFilters.DirectoryName
            //                     | NotifyFilters.FileName
            //                     | NotifyFilters.LastAccess
            //                     | NotifyFilters.LastWrite
            //                     | NotifyFilters.Security
            //                     | NotifyFilters.Size;

            FileSystemWatcher.Changed += OnChangedEvent;
            FileSystemWatcher.Created += OnCreatedEvent;

            FileSystemWatcher.Deleted += OnDeletedEvent;
            FileSystemWatcher.Renamed += OnRenamedEvent;
            FileSystemWatcher.Error += OnErrorEvent;



            //FileSystemWatcher.Filter = "**/" + RootAssetFolder.AssetManager.AssetFileName;
            //Console.WriteLine(FileSystemWatcher.Filter);

            FileSystemWatcher.IncludeSubdirectories = true;
            
            FileSystemWatcher.EnableRaisingEvents = true;

        }

        public void Dispose()
        {
            FileSystemWatcher.Dispose();
            SendEvents();
        }

        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            if (TimeElapsed > UpdateTime)
            {
                TimeElapsed = 0;

                SendEvents();



            }
        }

        protected void SendEvents()
        {
            while (FilesDeleted.Count > 0)
            {
                string fileDeleted = FilesDeleted[0];
                this.OnFileDeleted?.Invoke(RootAssetDirectory, fileDeleted);
                FilesDeleted.RemoveAt(0);
            }

            while (FilesCreated.Count > 0)
            {
                string fileCreated = FilesCreated[0];
                this.OnFileCreated?.Invoke(RootAssetDirectory, fileCreated);
                FilesCreated.RemoveAt(0);
            }
        }

        protected void OnUpdate()
        {
            this.TimeElapsed = 0f;
        }

        private void OnChangedEvent(object sender, FileSystemEventArgs e)
        {

            OnUpdate();
            
            Console.WriteLine("File changed: " + e.Name);

        }

        
        private void OnCreatedEvent(object sender, FileSystemEventArgs e)
        {
            OnUpdate();
            Console.WriteLine("File created: " + e.Name);
            if (FilesCreated.Contains(e.Name) == false)
            {
                FilesCreated.Add(e.Name);
            }

            
        }
        
        private void OnDeletedEvent(object sender, FileSystemEventArgs e)
        {
            OnUpdate();
            Console.WriteLine("File deleted: " + e.Name);
            if (FilesDeleted.Contains(e.Name) == false)
            {
                FilesDeleted.Add(e.Name);
            }
        }

        private void OnRenamedEvent(object sender, RenamedEventArgs e)
        {
            OnUpdate();
            Console.WriteLine("File renamed: " + e.Name);
        }

        private void OnErrorEvent(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("File error: " + e.GetException());
        }
    }
}
