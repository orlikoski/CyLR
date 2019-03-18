using System;
using System.IO;

namespace CyLR.archive
{
    public struct File
    {
        public readonly string Name;
        public readonly Stream Stream;
        public readonly DateTimeOffset Timestamp;

        public File(string name, Stream stream, DateTimeOffset timestamp)
        {
            Name = name;
            Stream = stream;
            Timestamp = timestamp;
        }
    }
}