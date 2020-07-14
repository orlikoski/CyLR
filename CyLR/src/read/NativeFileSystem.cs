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

        public Stream OpenFile(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

        public IEnumerable<string> GetFilesFromDir(string path, DirectoryInfo directory)
        {
            IEnumerable<DirectoryInfo> directoryInfos;
            try
            {
                directoryInfos = directory.GetDirectories();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Failed to read files in '{0}' due to insufficient privilages.", path);
                directoryInfos = Enumerable.Empty<DirectoryInfo>();
            }

            foreach (
                var file in
                    directoryInfos.SelectMany(subDir => GetFilesFromDir(Path.Combine(path, subDir.Name), subDir)))
            {
                yield return file;
            }
            IEnumerable<FileInfo> fileList;
            try
            {
                fileList = directory.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Failed to read files in '{0}' due to insufficient privilages.", path);
                fileList = Enumerable.Empty<FileInfo>();
            }

            if (!fileList.Any())
            {
                Console.WriteLine($"Folder '{path}' exists but contains no files");
            }
            foreach (var file in fileList)
            {
                yield return Path.Combine(path, file.Name);
            }
        }
    }
}