using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using DiscUtils;

namespace CyLR.write
{
    internal static class Archive
    {
      public static ZipArchive GetArchive(Stream destinationStream)
      {
        return new ZipArchive(destinationStream, ZipArchiveMode.Create, true);
      }

        public static void CollectFilesToArchive(this IEnumerable<Tuple<char, DiscFileInfo>> files, ZipArchive archive)
        {
                foreach (var file in files)
                {
                    WriteFileToArchive(archive, $@"{file.Item1}\{file.Item2.FullName}", file.Item2);
                }
        }

        private static void WriteFileToArchive(ZipArchive archive, string entryName, DiscFileInfo file)
        {
            string tmptext = entryName.Substring(0, 1) + ":" + entryName.Substring(1);
            Console.WriteLine("Collecting File: {0}", tmptext);
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                WriteStreamToArchive(archive, entryName, stream);
            }
        }

        private static void WriteStreamToArchive(ZipArchive archive, string entryName, Stream stream)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
            using (var writer = entry.Open())
            {
                stream.CopyTo(writer);
            }
        }
    }
}
