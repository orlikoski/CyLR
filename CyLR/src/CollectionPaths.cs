using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CyLR
{
    internal static class CollectionPaths
    {
        public static List<string> GetPaths(Arguments arguments, List<string> additionalPaths)
        {
            var defaultPaths = new List<string>
            {
                        @"C:\Windows\System32\config",
                        @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                        @"C:\Windows\Prefetch",
                        @"C:\Windows\Tasks",
                        @"C:\Windows\SchedLgU.Txt",
                        @"C:\Windows\System32\winevt\logs",
                        @"C:\Windows\System32\drivers\etc\hosts",
                        @"C:\$MFT"
            };
            if (Platform.IsUnixLike())
            {
                defaultPaths = new List<string>
                {
                    "/root/.bash_history",
                    "/var/logs"
                };
            }

            var paths = new List<string>(additionalPaths);
 
            if (arguments.CollectionFilePath != ".")
            {
                if (File.Exists(arguments.CollectionFilePath))
                {
                    paths.AddRange(File.ReadAllLines(arguments.CollectionFilePath));
                }
                else
                {
                    Console.WriteLine("Error: Could not find file: {0}", arguments.CollectionFilePath);
                    Console.WriteLine("Exiting");
                    throw new ArgumentException();
                }
            }

            if (arguments.CollectionFiles != null)
            {
                paths.AddRange(arguments.CollectionFiles);
            }

            return paths.Any() ? paths : defaultPaths;
        }
    }
}
