using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                var dirInfo = new DirectoryInfo(path);
                foreach (var file in GetFilesFromDir(path, dirInfo))
                {
                    yield return file;
                }
            }
            else
            {
                Console.WriteLine($"File or folder '{path}' does not exist");
            }
        }

        public IEnumerable<string> GetFilesFromDir(string path, DirectoryInfo directory)
        {
            foreach (var subDir in directory.GetDirectories())
            {
                foreach (var file in GetFilesFromDir(Path.Combine(path, subDir.Name), subDir))
                {
                    yield return file;
                }
            }
            var filelist = directory.GetFiles();
            if (!filelist.Any())
            {
                Console.WriteLine($"Folder '{path}' exists but contains no files");
            }
            foreach (var file in filelist)
            {
                yield return Path.Combine(path, file.Name);
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
