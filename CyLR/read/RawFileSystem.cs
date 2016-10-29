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
        private readonly IRawFileSystem system;

        public RawFileSystem(char driveLetter)
        {
            var disk = new RawDisk(driveLetter);
            var rawDiskStream = disk.CreateDiskStream();
            system = new NtfsFileSystem(rawDiskStream);
        }

        public IEnumerable<string> GetFilesFromPath(string path)
        {
            if (system.FileExists(path))
            {
                yield return system.GetFileInfo(path).FullName;
            }
            else if (system.DirectoryExists(path))
            {
                foreach (var fileInfo in system.GetDirectoryInfo(path).GetFiles())
                {
                    yield return fileInfo.FullName;
                }
            }
            else
            {
                Console.WriteLine($"File or folder '{path}' does not exist.");
            }
        }


        public Stream OpenFile(string path)
        {
            return system.OpenFile(path, FileMode.Open, FileAccess.Read);
        }
    }
}