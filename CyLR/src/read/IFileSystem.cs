using System;
using System.Collections.Generic;
using System.IO;

namespace CyLR.read
{
    internal interface IFileSystem
    {
        IEnumerable<string> GetFilesFromPath(string path);
        Stream OpenFile(string path);
        DateTime GetLastWriteTimeUtc(string path);
        DateTime GetLastWriteTime(string path);
        bool FileExists(string path);
    }
}