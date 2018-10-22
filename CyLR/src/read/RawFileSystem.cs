using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CyLR.src.read;
using DiscUtils;
using DiscUtils.Ntfs;
using RawDiskLib;
using IRawFileSystem = DiscUtils.IFileSystem;

namespace CyLR.read
{
    internal class RawFileSystem : IFileSystem
    {
        private readonly Dictionary<char, IRawFileSystem> systems = new Dictionary<char, IRawFileSystem>();

        public IEnumerable<string> GetFilesFromPath(string path)
        {
            var system = GetSystem(path);
            var letterlessPath = FullPathToRawPath(path);

            if (system.FileExists(letterlessPath))
            {
                yield return path;
            }
            else if (system.DirectoryExists(letterlessPath))
            {
                var dirInfo = system.GetDirectoryInfo(letterlessPath);
                // Grap all files in directory
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

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return GetSystem(path).GetLastWriteTimeUtc(FullPathToRawPath(path));
        }

        public DateTime GetLastWriteTime(string path)
        {
            return GetSystem(path).GetLastWriteTime(FullPathToRawPath(path));
        }

        public bool FileExists(string path)
        {
            return GetSystem(path).Exists(FullPathToRawPath(path));
        }

        public Stream OpenFile(string path)
        {
            var system = GetSystem(path);
            return system.OpenFile(FullPathToRawPath(path), FileMode.Open, FileAccess.Read);
        }

        private IRawFileSystem GetSystem(string path)
        {
            IRawFileSystem system;
            var driveLetter = path[0];

            if (!char.IsLetter(driveLetter))
            {
                throw new ArgumentException($"Path '{path}' did not have a drive letter!");
            }

            if (systems.TryGetValue(driveLetter, out system)) return system;

            try
            {
                var disk = new RawDisk(driveLetter);
                var rawDiskStream = disk.CreateDiskStream();
                system = new NtfsFileSystem(rawDiskStream);
                systems.Add(driveLetter, system);
            }
            catch (Exception e)
            {
                throw new DiskReadException($"Failed to create a filesystem for drive '{driveLetter}'", e);
            }
            return system;
        }

        private string FullPathToRawPath(string path)
        {
            return path.Substring(3); //chop off drive letter and leading slash
        }

        private IEnumerable<string> GetFilesFromDir(string path, DiscDirectoryInfo directory)
        {
            foreach (var subDir in directory.GetDirectories())
            {
                foreach (var file in GetFilesFromDir(Path.Combine(path, subDir.Name), subDir))
                {
                    yield return file;
                }
            }
            var fileList = directory.GetFiles();
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