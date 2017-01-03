#if !DOT_NET_4_0
using System;
using System.IO;
using System.IO.Compression;

namespace CyLR.archive
{
    class NativeArchive : Archive
    {
        private readonly ZipArchive archive;
        public NativeArchive(Stream destination)
            : base(destination)
        {
            archive = new ZipArchive(destination, ZipArchiveMode.Create, true);
        }

        protected override void WriteStreamToArchive(string entryName, Stream stream, DateTimeOffset timestamp)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
            entry.LastWriteTime = timestamp;
            using (var writer = entry.Open())
            {
                stream.CopyTo(writer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                archive.Dispose();
            }
        }
    }
}
#endif