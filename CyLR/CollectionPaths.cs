using System;
using PythLR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CyLR
{
    static class CollectionPaths
    {
        public static string [] GetPaths(Arguments arguments)
        {
            string[] paths =
{
            @"\Windows\System32\config",
            @"\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
            @"\Windows\Prefetch",
            @"\Windows\Tasks",
            @"\Windows\SchedLgU.Txt",
            @"\Windows\System32\winevt\logs",
            @"\Windows\System32\drivers\etc\hosts",
            @"$MFT"
            };

            if (arguments.CollectionFilePath != null)
            {
                if (File.Exists(arguments.CollectionFilePath))
                {
                    paths = File.ReadAllLines(arguments.CollectionFilePath);
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
