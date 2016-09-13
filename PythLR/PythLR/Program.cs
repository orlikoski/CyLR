using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using DiscUtils.Ntfs;
using RawDiskLib;

namespace PythLR
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string[] paths =
            {
                @"\Windows\System32\config",
                @"\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                @"\Windows\Prefetch",
                @"\Windows\Tasks",
                @"\Windows\SchedLgU.Txt",
                @"\Windows\System32\winevt\logs",
                @"\Windows\System32\drivers\etc\hosts",
                @"$MFT"
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var system = GetFileSystem('C');

            var outputPath = args.HasArgument("-o") ? args.GetArgumentParameter("-o") : ".";
            var zipPath = $"{outputPath}\\{Environment.MachineName}.zip";
            CollectFilesToArchive(paths, system, zipPath);

            stopwatch.Stop();
            Console.WriteLine("{0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }

        private static void CollectFilesToArchive(IEnumerable<string> paths, NtfsFileSystem system, string archivePath)
        {
            var archiveFile = new FileInfo(archivePath);
            Directory.CreateDirectory(archiveFile.Directory.FullName);
            using (var outStream = File.OpenWrite(archivePath))
            using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                foreach (var path in paths)
                {
                    var directory = system.GetDirectoryInfo(path);
                    if (system.FileExists(path))
                    {
                        WriteFileToArchive(system, zipStream, path.Substring(1));
                    }
                    else if (directory.Exists)
                    {
                        var files = directory.GetFiles();

                        foreach (var file in files)
                        {
                            WriteFileToArchive(system, zipStream, file.FullName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Directory '{0}' does not exist and has been skipped.", path);
                    }
                }
                WriteFileToArchive(system, zipStream,);
            }
        }

        private static void WriteFileToArchive(NtfsFileSystem system, ZipArchive zipStream, string file)
        {
            Console.WriteLine("Collecting File: {0}", file);
            using (var stream = system.OpenFile(file, FileMode.Open, FileAccess.Read))
            {
                WriteStreamToArchive(zipStream, file, stream);
            }
        }

        private static NtfsFileSystem GetFileSystem(char driveLetter)
        {
            var disk = new RawDisk(driveLetter);
            var rawDiskStream = disk.CreateDiskStream();
            var system = new NtfsFileSystem(rawDiskStream);
            return system;
        }

        private static void WriteStreamToArchive(ZipArchive zipStream, string entryName, Stream stream)
        {
            var entry = zipStream.CreateEntry(entryName, CompressionLevel.Fastest);
            using (var writer = entry.Open())
            {
                stream.CopyTo(writer);
            }
        }
    }
}