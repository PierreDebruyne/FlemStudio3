using FlemStudio.AssetManagement.Core.AssetDirectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.AssetManagement.Core
{
    public class AssetPath
    {
        protected AssetManager AssetManager;
        public string Value { get; }
        public string? Root { get; }
        public string Path { get; }

        public AssetPath(string path)
        {
            path = FormatAssetPath(path);

            string[] pathItems = path.Split(":/");
            if (pathItems.Length == 0 || pathItems.Length > 2)
            {
                throw new Exception("Asset path must be in format: 'rootFolderName:/path/to/asset'");
            }
            if (pathItems.Length == 1)
            {
                Root = null;
                Path = pathItems[0];
            } else
            {
                Root = pathItems[0];
                Path = pathItems[1];
            }
            
        }

        internal string FormatAssetPath(string path)
        {
            path = path.Replace('\\', '/');
            path = path.Replace("//", "/");
            while (path.Last() == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }
    }
}
