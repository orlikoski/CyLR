using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Ntfs;
using RawDiskLib;

using IRawFileSystem = DiscUtils.IFileSystem;

namespace CyLR.read
{
    internal class RawFileSystem : IFileSystem
    {
        private readonly Dictionary<char, IRawFileSystem> systems = new Dictionary<char, IRawFileSystem>();
        
        IRawFileSystem GetSystem(string path)
        {
            IRawFileSystem system;
            var driveLetter = path[0];

            if (!char.IsLetter(driveLetter))
            {
                throw new ArgumentException($"Path '{path}' did not have a drive letter!");
            }

            if (systems.TryGetValue(driveLetter, out system)) return system;

            var disk = new RawDisk(driveLetter);
            var rawDiskStream = disk.CreateDiskStream();
            system = new NtfsFileSystem(rawDiskStream);
            systems.Add(driveLetter, system);
            return system;
        }

        string FullPathToRawPath(string path)
        {
            return path.Substring(3); //chop off drive letter and leading slash
        }

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
                foreach (var fileInfo in system.GetDirectoryInfo(letterlessPath).GetFiles())
                {
                    yield return Path.Combine(path, fileInfo.Name);
                }
            }
            else
            {
                Console.WriteLine($"File or folder '{path}' does not exist.");
            }
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
    }
}