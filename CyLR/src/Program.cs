using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CyLR.archive;
using CyLR.read;
using CyLR.src.read;
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
                Console.Error.WriteLine(e.Message);
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unknown error while parsing arguments: {e.Message}");
                return 0;
            }

            if (arguments.HelpRequested)
            {
                Console.WriteLine(arguments.GetHelp(arguments.HelpTopic));
                return 0;
            }

            // Configure logging
            var logger = new CyLR.Logger();
            if (arguments.LogFilePath.Length > 0)
            {
                logger.LoggingOptions["output_file_path"] = arguments.LogFilePath;
            }
            if (arguments.EnableVerboseConsole)
            {
                logger.LoggingOptions["output_console_min_level"] = "debug";
            }
            if (arguments.DisableLogging)
            {
                logger.LoggingOptions["output_file_enabled"] = "false";
                logger.LoggingOptions["output_console_enabled"] = "false";
                logger.LoggingOptions["output_buffer_enabled"] = "false";
            }
            logger.Setup();
            
            // Enumerate arguments
            logger.debug(String.Format("Argument {0} is set to {1}", "OutputPath", arguments.OutputPath));
            logger.debug(String.Format("Argument {0} is set to {1}", "OutputFileName", arguments.OutputFileName));
            logger.debug(String.Format("Argument {0} is set to {1}", "UserName", arguments.UserName));
            logger.debug(String.Format("Argument {0} is set to {1}", "UserPassword", "[omitted in log file]"));
            logger.debug(String.Format("Argument {0} is set to {1}", "SFTPServer", arguments.SFTPServer));
            logger.debug(String.Format("Argument {0} is set to {1}", "SFTPOutputPath", arguments.SFTPOutputPath));
            logger.debug(String.Format("Argument {0} is set to {1}", "SFTPCleanUp", arguments.SFTPCleanUp));
            logger.debug(String.Format("Argument {0} is set to {1}", "CollectionFilePath", arguments.CollectionFilePath));
            logger.debug(String.Format("Argument {0} is set to {1}", "CollectDefaults", arguments.CollectDefaults));
            logger.debug(String.Format("Argument {0} is set to {1}", "ZipPassword", "[omitted in log file]"));
            logger.debug(String.Format("Argument {0} is set to {1}", "ZipLevel", arguments.ZipLevel));
            logger.debug(String.Format("Argument {0} is set to {1}", "LogFilePath", arguments.LogFilePath));
            logger.debug(String.Format("Argument {0} is set to {1}", "DisableLogging", arguments.DisableLogging));
            logger.debug(String.Format("Argument {0} is set to {1}", "EnableVerboseConsole", arguments.EnableVerboseConsole));
            logger.debug(String.Format("Argument {0} is set to {1}", "Usnjrnl", arguments.Usnjrnl));
            logger.debug(String.Format("Argument {0} is set to {1}", "ForceNative", arguments.ForceNative));
            logger.debug(String.Format("Argument {0} is set to {1}", "DryRun", arguments.DryRun));
            logger.debug(String.Format("Argument {0} is set to {1}", "CollectionFiles", arguments.CollectionFiles));

            var additionalPaths = new List<string>();
            if (Platform.IsInputRedirected)
            {
                string input = null;
                while ((input = Console.In.ReadLine()) != null)
                {
                    input = Environment.ExpandEnvironmentVariables(input);
                    additionalPaths.Add(input);
                }
                logger.debug("Identified additional collection paths from STDIN");
            }

            List<string> paths;
            try
            {
                logger.info("Gathering paths to collect");
                paths = CollectionPaths.GetPaths(arguments, additionalPaths, arguments.Usnjrnl, logger);
                logger.info(String.Format("{0} paths identified for collection", paths.Count));
            }
            catch (Exception e)
            {
                logger.error($"Error occurred while collecting files:\n{e}");
                logger.TearDown();
                return 1;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var archiveStream = Stream.Null;
                var outputPath = $@"{arguments.OutputPath}/{arguments.OutputFileName}";
                logger.debug(String.Format("Set outputPath to {0}", outputPath));
                if (!arguments.DryRun)
                {
                    logger.debug("Initializing archive file");
                    archiveStream = OpenFileStream(outputPath);
                }
                using (archiveStream)
                {
                    logger.info("Adding files to archive");
                    CreateArchive(arguments, archiveStream, paths, logger);
                }

                stopwatch.Stop();
                logger.info(String.Format("Collection complete. {0} elapsed", new TimeSpan(stopwatch.ElapsedTicks).ToString("g")));

                if (arguments.UseSftp)
                {
                    // Attempt upload of SFTP.
                    logger.debug("Start SFTP Upload");
                    SFTPUpload(arguments, outputPath, logger);
                }
            }
            catch (Exception e)
            {
                logger.error($"Error occurred while collecting files:\n{e}");
                logger.TearDown();
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Handle the connection to the SFTP server and uploading the resulting
        /// archive file. In the case the upload fails, this method will attempt
        /// to re-upload 3 times, with a 30 second pause between to allow time
        /// for the network to become more stable. If the upload is successful,
        /// the resulting archive file will be removed from the system - unless
        /// the user specified <c>--no-sftpcleanup</c> at invocation.
        /// </summary>
        /// <param name="arguments">User specified arguments with SFTP and other details</param>
        /// <param name="outputPath">Path to the archive file to upload</param>
        /// <param name="logger">Logging object</param>
        private static void SFTPUpload(Arguments arguments, string outputPath, Logger logger){
            logger.info("Uploading ZIP to SFTP");
            bool successfulUpload = false;
            int max_tries = 3;
            int num_tries = 0;
            while (!successfulUpload && (num_tries < max_tries))
            {
                bool attemptSuccess = false;
                try
                {

                    var sftpStream = Stream.Null;
                    var client = CreateSftpClient(arguments);
                    sftpStream = client.Create($@"{arguments.SFTPOutputPath}/{arguments.OutputFileName}");

                    const int bufferSize = 1048576;
                    byte[] buffer = new byte[1048576];
                    int readSize = -1;
                    ulong amountCopied = 0;
                    ulong pctComplete = 0;

                    using (sftpStream)
                    using (FileStream sr = File.OpenRead(outputPath))
                    {
                        do {
                            readSize = sr.Read(buffer, 0, bufferSize);
                            if (readSize > 0) 
                            {
                                sftpStream.Write(buffer, 0, readSize);
                            }
                            amountCopied += (ulong)readSize;
                            if (readSize > 0 && (amountCopied % (1048576*50)) == 0)
                            {
                                pctComplete = ((ulong)amountCopied*100) / (ulong)sr.Length;
                                logger.info(String.Format("Read {0}%, {1}", pctComplete, String.Format("{0:n0} MB", (amountCopied/(1048576)))));
                            }
                        } while (readSize > 0);
                        if (readSize > 0 && (amountCopied % (1048576*50)) == 0)
                        {
                            pctComplete = ((ulong)amountCopied*100) / (ulong)sr.Length;
                            logger.info(String.Format("Read {0}%, {1}", pctComplete, String.Format("{0:n0} MB", (amountCopied/(1048576)))));
                        }
                    }
                    attemptSuccess = true;

                    Task.Factory.StartNew(() => {
                        client.Dispose();
                    });

                    logger.info("SFTP Upload complete.");

                }
                catch
                {
                    logger.warn(String.Format("Upload failed. Retrying {0} more times", max_tries - num_tries));
                    logger.warn("Sleeping for 30 seconds");
                    num_tries++;
                    System.Threading.Thread.Sleep(30*1000);
                }

                if (attemptSuccess){
                    successfulUpload = true;
                    logger.info("Upload complete.");
                    if (arguments.SFTPCleanUp){
                        File.Delete(outputPath);
                    }
                    logger.info("Removed local zip file collection.");
                                        
                }

            }
            if (!successfulUpload){
                logger.warn("Unable to upload to SFTP. Zip file not removed. Please upload through another manner.");
            }
            
        }

        /// <summary>
        ///     Creates a zip archive containing all files from provided paths.
        /// </summary>
        /// <param name="arguments">Program arguments.</param>
        /// <param name="archiveStream">The Stream the archive will be written to.</param>
        /// <param name="paths">Map of driveLetter->path for all files to collect.</param>
        private static void CreateArchive(Arguments arguments, Stream archiveStream, IEnumerable<string> paths, Logger logger)
        {
            try
            {
                string ZipLevel = "3";
                if (!String.IsNullOrEmpty(arguments.ZipLevel))
                {
                   ZipLevel = arguments.ZipLevel;
                   logger.debug(String.Format("Set zip compression level to {0}", ZipLevel));
                }
                using (var archive = new SharpZipArchive(archiveStream, arguments.ZipPassword, ZipLevel ))
                {
                    var system = arguments.ForceNative ? (IFileSystem)new NativeFileSystem() : new RawFileSystem();

                    var filePaths = paths.SelectMany(path => system.GetFilesFromPath(path)).ToList();
                    foreach (var filePath in filePaths.Where(path => !system.FileExists(path)))
                    {
                        logger.warn($"Warning: file or folder '{filePath}' does not exist.");
                    }
                    var fileHandles = OpenFiles(system, filePaths, logger);

                    archive.CollectFilesToArchive(fileHandles);

                    // Save the active log message to the archive file prior to completion
                    // though after the collection of targeted files finishes.
                    ArchiveCurrentLog(logger, archive, system);
                }
            }
            catch(DiskReadException e)
            {
                logger.error($"Failed to read files, this is usually due to lacking admin privileges.\nError:\n{e}");
            }
        }

        /// <summary>Caches current log messages within the Zip archive</summary>
        /// <param name="logger">The logging object</param>
        /// <param name="archive">The destination archive object</param>
        /// <param name="system">The system object used to prep the log file for export</param>
        private static void ArchiveCurrentLog(Logger logger, SharpZipArchive archive, IFileSystem system)
        {
            // Only run if logging is enabled
            if (logger.LoggingOptions["output_buffer_enabled"] == "true")
            {
                string logfileTempPath = String.Format("CyLR_Collection_Log_{0}.log", DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss"));
                // Cache log messages to File object
                using (StreamWriter sw = File.CreateText(logfileTempPath))
                {
                    sw.Write(logger.logMessages);
                }

                // Add cached file to archive
                List<string> paths = new List<string>{logfileTempPath};
                var fileHandles = OpenFiles(system, paths, logger);
                archive.CollectFilesToArchive(fileHandles);

                // Removed cached log file
                File.Delete(logfileTempPath);                
            }
        }

        private static IEnumerable<ArchiveFile> OpenFiles(IFileSystem system, IEnumerable<string> files, Logger logger)
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
                        logger.error($"Error: {e.Message}");
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