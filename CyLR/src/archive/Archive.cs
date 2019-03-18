using System;
using System.Collections.Generic;
using System.IO;

namespace CyLR.archive
{
    public abstract class Archive : IDisposable
    {
        private readonly Stream destination;
        protected Archive(Stream destination)
        {
            this.destination = destination;
        }
        public void CollectFilesToArchive(IEnumerable<File> files)
        {
            foreach (var file in files)
            {
                WriteFileToArchive(file);
            }
        }

        private void WriteFileToArchive(File file)
        {
            Console.WriteLine($"Collecting File: {file.Name}");
            using (file.Stream)
            {
                WriteStreamToArchive(file.Name.Replace(":", ""), file.Stream, file.Timestamp);
            }
        }

        protected abstract void WriteStreamToArchive(string entryName, Stream stream, DateTimeOffset timestamp);

        #region IDisposable Support

        protected abstract void Dispose(bool disposing);

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            destination.Seek(0, SeekOrigin.Begin); //rewind the stream
        }

        #endregion IDisposable Support
    }
}