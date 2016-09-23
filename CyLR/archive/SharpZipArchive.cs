#if DOT_NET_4_0
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace CyLR.archive
{
    class SharpZipArchive : Archive
    {
        private readonly ZipOutputStream archive;

        public SharpZipArchive(Stream destination)
            : base(destination)
        {
            archive = new ZipOutputStream(destination);
        }

        protected override void WriteStreamToArchive(string entryName, Stream stream)
        {
            var entry = new ZipEntry(entryName);
            archive.PutNextEntry(entry);
            StreamUtils.Copy(stream, archive, new byte[4096]);
            archive.CloseEntry();
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