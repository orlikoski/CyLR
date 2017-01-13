using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CyLR.archive;
using CyLR.read;
using Renci.SshNet;
using ArchiveFile = CyLR.archive.File;
using File = System.IO.File;

namespace CyLR
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Arguments arguments;
            try
            {
                arguments = new Arguments(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error while parsing arguments: {e.Message}");
                return 0;
            }

            if (arguments.HelpRequested)
            {
                Console.WriteLine(arguments.GetHelp(arguments.HelpTopic));
                return 0;
            }

            var additionalPaths = new List<string>();
            if (Platform.IsInputRedirected)
            {
                string input = null;
                while ((input = Console.ReadLine()) != null)
                {
                    additionalPaths.Add(input);
                }
            }


            List<string> paths;
            try
            {
                paths = CollectionPaths.GetPaths(arguments, additionalPaths);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while collecting files:\n{e}");
                return 1;
            }


            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var archiveStream = Stream.Null;
                if (!arguments.DryRun)
                {
                    var outputPath = $@"{arguments.OutputPath}/{arguments.OutputFileName}";
                    if (arguments.UseSftp)
                    {
                        var client = CreateSftpClient(arguments);
                        archiveStream = client.Create(outputPath);
                    }
                    else
                    {
                        archiveStream = OpenFileStream(outputPath);
                    }
                }
                using (archiveStream)
                {
                    CreateArchive(arguments, archiveStream, paths);
                }

                stopwatch.Stop();
                Console.WriteLine("Extraction complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while collecting files:\n{e}");
                return 1;
            }
            return 0;
        }

        /// <summary>
        ///     Creates a zip archive containing all files from provided paths.
        /// </summary>
        /// <param name="arguments">Program arguments.</param>
        /// <param name="archiveStream">The Stream the archive will be written to.</param>
        /// <param name="paths">Map of driveLetter->path for all files to collect.</param>
        private static void CreateArchive(Arguments arguments, Stream archiveStream, IEnumerable<string> paths)
        {
#if DOT_NET_4_0
            using (var archive = new SharpZipArchive(archiveStream))
#else
            using (var archive = new NativeArchive(archiveStream))
#endif
            {
                var system = arguments.ForceNative ? (IFileSystem) new NativeFileSystem() : new RawFileSystem();

                var filePaths = paths.SelectMany(path => system.GetFilesFromPath(path)).ToList();
                foreach (var filePath in filePaths.Where(path => !system.FileExists(path)))
                {
                    Console.WriteLine($"Warning: file or folder '{filePath}' does not exist.");
                }
                var fileHandles = OpenFiles(system, filePaths);

                archive.CollectFilesToArchive(fileHandles);
            }
        }

        private static IEnumerable<ArchiveFile> OpenFiles(IFileSystem system, IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (system.FileExists(file))
                {
                    Stream stream = null;
                    try
                    {
                        stream = system.OpenFile(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    if (stream != null)
                    {
                        yield return new ArchiveFile(file, stream, system.GetLastWriteTime(file));
                    }
                }
            }
        }

        /// <summary>
        ///     Create an SFTP client and connect to a server using configuration from the arguments.
        /// </summary>
        /// <param name="arguments">The arguments to use to connect to the SFTP server.</param>
        private static SftpClient CreateSftpClient(Arguments arguments)
        {
            int port;
            var server = arguments.SFTPServer.Split(':');
            try
            {
                port = int.Parse(server[1]);
            }
            catch (Exception)
            {
                port = 22;
            }

            var client = new SftpClient(server[0], port, arguments.UserName, arguments.UserPassword);
            client.Connect();
            return client;
        }

        /// <summary>
        ///     Opens a file for reading and writing, creating any missing directories in the path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The file Stream.</returns>
        private static Stream OpenFileStream(string path)
        {
            var archiveFile = new FileInfo(path);
            if (archiveFile.Directory != null && !archiveFile.Directory.Exists)
            {
                archiveFile.Directory.Create();
            }
            return File.Open(archiveFile.FullName, FileMode.Create, FileAccess.ReadWrite);
        }
    }
}