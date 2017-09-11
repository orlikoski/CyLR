using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CyLR
{
    internal static class CollectionPaths
    {
        private static IEnumerable<string> RunCommand(string OSCommand, string CommandArgs)
        {
            var newPaths = new List<string> { };
            var proc = new Process
            { 
                StartInfo = new ProcessStartInfo
                {
                    FileName = OSCommand,
                    Arguments = CommandArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                yield return  proc.StandardOutput.ReadLine();
            };
        }
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
                    "/var/log",
                    "/private/var/log/",
                    "/.fseventsd",
                    "/etc/hosts.allow",
                    "/etc/hosts.deny",
                    "/etc/hosts",
                    "/System/Library/StartupItems",
                    "/System/Library/LaunchAgents",
                    "/System/Library/LaunchDaemons",
                    "/Library/LaunchAgents",
                    "/Library/LaunchDaemons",
                    "/Library/StartupItems",
                    "/etc/passwd",
                    "/etc/group"
                };
                // Find all *.plist files
                defaultPaths.AddRange(RunCommand("/usr/bin/find", "/ -name \"*.plist\" -print"));
                // Find all .bash_history files
                defaultPaths.AddRange(RunCommand("/usr/bin/find", "/ -name \".bash_history\" -print"));
                // Find all .sh_history files
                defaultPaths.AddRange(RunCommand("/usr/bin/find", "/ -name \".sh_history\" -print"));
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
