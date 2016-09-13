using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using DiscUtils;

namespace PythLR
{
    internal static class Archive
    {
        public static void CollectFilesToArchive(this IEnumerable<DiscFileInfo> files,
            string archivePath)
        {
            var archiveFile = new FileInfo(archivePath);
            Directory.CreateDirectory(archiveFile.Directory.FullName);
            using (var outStream = File.OpenWrite(archivePath))
            using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    WriteFileToArchive(zipStream, file);
                }
            }
        }

        private static void WriteFileToArchive(ZipArchive zipStream, DiscFileInfo file)
        {
            Console.WriteLine("Collecting File: {0}", file.FullName);
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                WriteStreamToArchive(zipStream, file.FullName, stream);
            }
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