#if !DOT_NET_4_0
using System.IO;
using System.IO.Compression;

namespace CyLR.archive
{
    class NativeArchive : Archive
    {
        private readonly ZipArchive archive;
        public NativeArchive(Stream destination)
        {
            archive = new ZipArchive(destination, ZipArchiveMode.Create, true);
        }

        protected override void WriteStreamToArchive(string entryName, Stream stream)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
            using (var writer = entry.Open())
            {
                stream.CopyTo(writer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            archive.Dispose();
        }
    }
}
#endif