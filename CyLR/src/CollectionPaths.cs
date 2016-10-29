using System;
using System.IO;
using System.Collections.Generic;

namespace CyLR
{
    internal static class CollectionPaths
    {
        public static List<string> GetPaths(Arguments arguments)
        {
            var paths = new List<string>
            {
                        @"C:\Windows\System32\config",
                        @"C:\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                        @"C:\Windows\Prefetch",
                        @"C:\Windows\Tasks",
                        @"C:\Windows\SchedLgU.Txt",
                        @"C:\Windows\System32\winevt\logs",
                        @"C:\Windows\System32\drivers\etc\hosts",
                        @"C:\$MFT"
            };
            if (Platform.IsUnixLike())
            {
                paths = new List<string>
                {
                    "/etc/hosts"
                };
            }
 
            if (arguments.CollectionFilePath != ".")
            {
                if (File.Exists(arguments.CollectionFilePath))
                {
                    paths.Clear();
                    paths.AddRange(File.ReadAllLines(arguments.CollectionFilePath));
                }
                else
                {
                    Console.WriteLine("Error: Could not find file: {0}", arguments.CollectionFilePath);
                    Console.WriteLine("Exiting");
                    throw new ArgumentException();
                }

            }

            return paths;
        }
    }
}
