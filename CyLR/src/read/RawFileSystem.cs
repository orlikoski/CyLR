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
            List<string> files = new List<string>();

            if (system.FileExists(letterlessPath))
            {
                files.Add(path);
            }
            else if (system.DirectoryExists(letterlessPath))
            {
                if (system.GetFiles(letterlessPath).Length == 0)
                {
                    Console.WriteLine($"Folder '{path}' exists but contains no files");
                }
                // Grap all files in directory
                foreach (var fileInfo in system.GetDirectoryInfo(letterlessPath).GetFiles())
                {
                    files.Add(Path.Combine(path, fileInfo.Name));
                }
                // Dive into all sub-directories
                foreach (var dirInfo in system.GetDirectories(letterlessPath))
                {
                    Console.WriteLine($"Made it: '{dirInfo}'");
                    files.AddRange(GetFilesFromPath(dirInfo));
                }

                foreach (var file in files) yield return file;
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
    }
}