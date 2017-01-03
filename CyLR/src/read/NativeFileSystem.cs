using System;
using System.Collections.Generic;
using System.IO;

namespace CyLR.read
{
    internal class NativeFileSystem : IFileSystem
    {
        public IEnumerable<string> GetFilesFromPath(string path)
        {
            if (File.Exists(path))
            {
                yield return path;
            }
            else if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    yield return file;
                }
            }
        }

        public Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
