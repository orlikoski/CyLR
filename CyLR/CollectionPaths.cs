using System;
using System.IO;
using System.Collections.Generic;

namespace CyLR
{
    static class CollectionPaths
    {
        public static Dictionary<char, List<string>> GetPaths(Arguments arguments)
        {
            var paths = new Dictionary<char, List<string>>
            {
                {'C',
                    new List<string>
                    {
                        @"\Windows\System32\config",
                        @"\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                        @"\Windows\Prefetch",
                        @"\Windows\Tasks",
                        @"\Windows\SchedLgU.Txt",
                        @"\Windows\System32\winevt\logs",
                        @"\Windows\System32\drivers\etc\hosts",
                        @"$MFT"
                    }
                }
            };
 
            if (arguments.CollectionFilePath != ".")
            {
                if (File.Exists(arguments.CollectionFilePath))
                {
                    paths.Clear();
                    var filepaths = File.ReadAllLines(arguments.CollectionFilePath);

                    foreach (string line in filepaths)
                    {
                        if (!paths.ContainsKey(line[0]))
                        {
                            paths.Add(line[0], new List<string>());
                        }
                        paths[line[0]].Add(line.Substring(2));
                    }
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
