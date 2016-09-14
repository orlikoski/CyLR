using System;
using System.Diagnostics;
using System.Linq;

namespace PythLR
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var arguments = new Arguments(args);

            if (arguments.HelpRequested)
            {
                Console.WriteLine(arguments.GetHelp(arguments.HelpTopic));
                return;
            }

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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var system = FileSystem.GetFileSystem('C');
            var zipPath = $@"{arguments.OutputPath}\{Environment.MachineName}.zip";
            var files = paths.SelectMany(path => system.GetFilesFromPath(path));



            if (arguments.SFTPCheck)
            {
                callsftpfunc();
            }
            else
            {
                files.CollectFilesToArchive(zipPath);
            }


            stopwatch.Stop();
            Console.WriteLine("Extraction complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }
    }
}