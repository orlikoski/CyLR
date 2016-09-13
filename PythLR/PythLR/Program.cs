using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using DiscUtils.Ntfs;
using RawDiskLib;

namespace PythLR
{
    static class Program
    {
        static void Main(string[] args)
        {
            string[] paths =
            {
                @"\Windows\System32\config",
                @"\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                @"\Windows\Prefetch",
                @"\Windows\Tasks",
                @"\Windows\SchedLgU.Txt",
                @"\Windows\System32\winevt\logs",
                @"\Windows\System32\drivers\etc\hosts"
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var system = GetFileSystem('C');

            var zipfilename = Environment.MachineName+".zip";
            CollectFilesToArchive(paths, system, zipfilename);

            stopwatch.Stop();
            Console.WriteLine("{0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }

        private static void CollectFilesToArchive(IEnumerable<string> paths, NtfsFileSystem system, string archivePath)
        {

            using (var outStream = File.OpenWrite(archivePath))
            using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                foreach (var path in paths)
                {
                    var directory = system.GetDirectoryInfo(path);
                    if (system.FileExists(path))
                    {
                        writefile(system, zipStream, path.Substring(1));
                    }
                    else if (directory.Exists)
                    {
                        var files = directory.GetFiles();

                        foreach (var file in files)
                        {
                            writefile(system, zipStream, file.FullName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Directory '{0}' does not exist and has been skipped.", path);
                    }
                }
                writefile(system, zipStream, @"$MFT");
            }
        }

        private static void writefile(NtfsFileSystem system, ZipArchive zipStream, String file)
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
