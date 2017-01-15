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
                        @"%SYSTEMROOT%\System32\drivers\etc\hosts",
                        @"%SYSTEMROOT%\SchedLgU.Txt",
                        @"%PROGRAMDATA%\Microsoft\Windows\Start Menu\Programs\Startup",
                        @"%SYSTEMROOT%\System32\config",
                        @"%SYSTEMROOT%\System32\winevt\logs",
                        @"%SYSTEMROOT%\Prefetch",
                        @"%SYSTEMROOT%\Tasks",
                        @"%SYSTEMROOT%\System32\LogFiles\W3SVC1",
                        @"%SystemDrive%\$MFT"
            };
            defaultPaths = defaultPaths.Select(Environment.ExpandEnvironmentVariables).ToList();

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
                    paths.AddRange(File.ReadAllLines(arguments.CollectionFilePath).Select(Environment.ExpandEnvironmentVariables));
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
