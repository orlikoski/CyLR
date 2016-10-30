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

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
