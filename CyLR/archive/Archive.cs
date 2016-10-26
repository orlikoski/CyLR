using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils;

namespace CyLR.archive
{
    internal abstract class Archive : IDisposable
    {
        private readonly Stream destination;
        protected Archive(Stream destination)
        {
            this.destination = destination;
        }
        public void CollectFilesToArchive(IEnumerable<Tuple<string, Stream>> files)
        {
            foreach (var file in files)
            {
                WriteFileToArchive($@"{file.Item1}", file.Item2);
            }
        }

        private void WriteFileToArchive(string entryName, Stream file)
        {
            var tmptext = entryName.Substring(0, 1) + ":" + entryName.Substring(1);
            Console.WriteLine("Collecting File: {0}", tmptext);
            using (file)
            {
                WriteStreamToArchive(entryName, file);
            }
        }

        protected abstract void WriteStreamToArchive(string entryName, Stream stream);

        #region IDisposable Support
        protected abstract void Dispose(bool disposing);

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            destination.Seek(0, SeekOrigin.Begin); //rewind the stream
        }
        #endregion
    }
}
