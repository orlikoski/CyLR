#if DOT_NET_4_0
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace CyLR.archive
{
    class SharpZipArchive : Archive
    {
        private readonly ZipOutputStream archive;

        public SharpZipArchive(Stream destination, String password)
            : base(destination)
        {
            archive = new ZipOutputStream(destination);
            archive.IsStreamOwner = false;
            if (!string.IsNullOrEmpty(password))
            {
                archive.Password = password;
            }
            
        }

        protected override void WriteStreamToArchive(string entryName, Stream stream, DateTimeOffset timestamp)
        {
            var entry = new ZipEntry(entryName)
            {
                DateTime = timestamp.DateTime
            };
            archive.PutNextEntry(entry);
            archive.SetLevel(3);

            stream.CopyTo(archive);
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