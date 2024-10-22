﻿using FlemStudio.AssetManagement.Core.AssetDirectories;
using FlemStudio.AssetManagement.Core.Assets;
using FlemStudio.AssetManagement.Core.RootAssetDirectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.Core
{
    public class AssetRegistryUpdater
    {
        protected AssetRegistry AssetRegistry;

        protected Dictionary<string, RootAssetFolderWatcher> Watchers = new();

        public AssetRegistryUpdater(AssetRegistry assetRegistry)
        {
            AssetRegistry = assetRegistry;
            AssetRegistry.OnRootAssetDirectoryAdded += OnRootAssetDirectoryAdded;
        }

        public void Dispose()
        {
            AssetRegistry.OnRootAssetDirectoryAdded -= OnRootAssetDirectoryAdded;

            foreach (var watcher in Watchers.Values)
            {
                watcher.Dispose();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var watcher in Watchers.Values)
            {
                watcher.Update(deltaTime);
            }
        }

        private void OnRootAssetDirectoryAdded(RootAssetDirectory directory)
        {
            RootAssetFolderWatcher watcher = new RootAssetFolderWatcher(directory);
            Watchers.Add(directory.Info.Name, watcher);
            watcher.OnFileCreated += OnFileCreated;
            watcher.OnFileDeleted += OnFileDeleted;
        }

        

        private void OnFileCreated(RootAssetDirectory rootDirectory, string path)
        {
            path = AssetManager.FormatAssetPath(path);
            Console.WriteLine("File created: " + rootDirectory.Info.Name + ":/" + path);

            if (path.EndsWith(AssetManager.AssetDirectoryDefinitionFileName))
            {
                AssetDirectoryInfo info = rootDirectory.Info.GetAssetDirectoryInfo(string.Join("/", path.Split("/").SkipLast(1)));
                AssetRegistry.RegisterAssetDirectory(info);
            } else if (path.EndsWith(AssetManager.AssetDefinitionFileName))
            {
                AssetInfo info = rootDirectory.Info.GetAssetInfo(string.Join("/", path.Split("/").SkipLast(1)));
                AssetRegistry.RegisterAsset(info);
            }
        }

        private void OnFileDeleted(RootAssetDirectory rootDirectory, string path)
        {
            path = AssetManager.FormatAssetPath(path);
            Console.WriteLine("File deleted: " + rootDirectory.Info.Name + ":/" + path);
            if (path.EndsWith(AssetManager.AssetDirectoryDefinitionFileName))
            {
                AssetDirectoryInfo info = rootDirectory.Info.GetAssetDirectoryInfo(string.Join("/", path.Split("/").SkipLast(1)));
                //AssetRegistry.UnregisterAssetDirectory(info);
            }
            else if (path.EndsWith(AssetManager.AssetDefinitionFileName))
            {
                AssetInfo info = rootDirectory.Info.GetAssetInfo(string.Join("/", path.Split("/").SkipLast(1)));
                //AssetRegistry.UnregisterAsset(info);
            }
        }

        
    }
}
