using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils;
using DiscUtils.Ntfs;
using RawDiskLib;

namespace CyLR.read
{
    internal static class FileSystem
    {
        public static IFileSystem GetFileSystem(char driveLetter, FileAccess fileAccess)
        {
            var disk = new RawDisk(driveLetter, fileAccess);
            var rawDiskStream = disk.CreateDiskStream();
            var system = new NtfsFileSystem(rawDiskStream);
            return system;
        }

        public static IEnumerable<DiscFileInfo> GetFilesFromPath(this IFileSystem system, string path)
        {
            if (system.FileExists(path))
            {
                yield return system.GetFileInfo(path);
            }
            else if (system.DirectoryExists(path))
            {
                foreach (var fileInfo in system.GetDirectoryInfo(path).GetFiles())
                {
                    yield return fileInfo;
                }
            }
            else
            {
                Console.WriteLine($"File or folder '{path}' does not exist.");
            }
        }
    }
}