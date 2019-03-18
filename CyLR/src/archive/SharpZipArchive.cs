using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace CyLR.archive
{
    public class SharpZipArchive : Archive
    {
        private readonly ZipOutputStream archive;

        public SharpZipArchive(Stream destination, String password, String level)
            : base(destination)
        {
            archive = new ZipOutputStream(destination);
            archive.IsStreamOwner = false;
            if (!string.IsNullOrEmpty(password))
            {
                archive.Password = password;
            }
            if(Convert.ToInt32(level) >=1 || Convert.ToInt32(level) <= 9)
            {
                archive.SetLevel(Convert.ToInt32(level));
            }
        }

        protected override void WriteStreamToArchive(string entryName, Stream stream, DateTimeOffset timestamp)
        {
            var entry = new ZipEntry(entryName)
            {
                DateTime = timestamp.DateTime
            };
            archive.PutNextEntry(entry);
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