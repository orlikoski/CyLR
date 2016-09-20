using System;
using System.Collections.Generic;
using DiscUtils;
using DiscUtils.Ntfs;
using RawDiskLib;

namespace PythLR
{
    internal static class FileSystem
    {
        public static IFileSystem GetFileSystem(char driveLetter)
        {
            var disk = new RawDisk(driveLetter);
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
                Console.WriteLine($"File or folder '{path}' does not exists.");
            }
        }
    }
}