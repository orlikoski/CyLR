using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DiscUtils;
using DiscUtils.Ntfs;
using PythLR.write;

namespace PythLR
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Arguments arguments;
            try
            {
                arguments = new Arguments(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error while parsing arguments: {e.Message}");
                return;
            }

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

            try
            {
                var system = FileSystem.GetFileSystem('C', FileAccess.Read);

                var files = paths.SelectMany(path => system.GetFilesFromPath(path));

                if (arguments.SFTPCheck)
                {
                    var archiveStream = arguments.SFTPInMemory ? new MemoryStream() : OpenFileStream(system, $@"{arguments.OutputPath}\{Environment.MachineName}.zip");
                    using (archiveStream)
                    {
                        files.CollectFilesToArchive(archiveStream);
                        Sftp.SendUsingSftp(archiveStream, arguments.SFTPServer, 22, arguments.UserName, arguments.UserPassword, $@"{arguments.OutputPath}/{Environment.MachineName}.zip");
                    }
                }
                else
                {
                    var zipPath = $@"{arguments.OutputPath}\{Environment.MachineName}.zip";
                    var archiveStream = OpenFileStream(system, zipPath);
                    using (archiveStream)
                    {
                        files.CollectFilesToArchive(archiveStream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while collecting files:\n{e}");
            }

            stopwatch.Stop();
            Console.WriteLine("Extraction complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
        }

        private static Stream OpenFileStream(IFileSystem system, string path)
        {
            var archiveFile = new FileInfo(path);
            if (archiveFile.Directory != null && !archiveFile.Directory.Exists)
            {
                archiveFile.Directory.Create();
            }
            return File.Create(archiveFile.FullName); //TODO: Replace with non-api call
        }
    }
}
