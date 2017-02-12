using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Ntfs;
using RawDiskLib;

using IRawFileSystem = DiscUtils.IFileSystem;
using DiscUtils;
using System.Linq;

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

        IEnumerable<string> GetFilesFromDir(string path, DiscDirectoryInfo directory)
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
    }
}