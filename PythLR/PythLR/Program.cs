using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PythLR.write;
using Renci.SshNet;

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

            var files = paths.SelectMany(path => system.GetFilesFromPath(path));



            if (arguments.SFTPCheck)
            {
                var zipPath = $@".\{Environment.MachineName}.zip";
                files.CollectFilesToArchive(zipPath);
                var client = new SftpClient(arguments.SFTPServer, 22, arguments.UserName, arguments.UserPassword);
                client.Connect();
                client.UploadFile(new FileStream(zipPath, FileMode.Open, FileAccess.Read), $@"{arguments.OutputPath}/{Environment.MachineName}.zip");
                client.Disconnect();
            }
            else
            {
                var zipPath = $@"{arguments.OutputPath}\{Environment.MachineName}.zip";
                files.CollectFilesToArchive(zipPath);
            }


            stopwatch.Stop();
            Console.WriteLine("Extraction complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }
    }
}