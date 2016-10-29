using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CyLR.read
{
    interface IFileSystem
    {
        IEnumerable<string> GetFilesFromPath(string path);
        Stream OpenFile(string path);
    }
}
