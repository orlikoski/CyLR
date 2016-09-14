using System;
using System.Diagnostics;
using System.Linq;

namespace PythLR
{
    internal static class Program
    {
        private static void Main(string[] args)
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var system = FileSystem.GetFileSystem('C');

            var outputPath = args.HasArgument("-o") ? args.GetArgumentParameter("-o") : ".";
            var zipPath = $@"{outputPath}\{Environment.MachineName}.zip";

            var files = paths.SelectMany(path => system.GetFilesFromPath(path));
            files.CollectFilesToArchive(zipPath);

            stopwatch.Stop();
            Console.WriteLine("Extraction complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }
    }
}