using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Win32;

namespace CyLR
{
    internal static class CollectionPaths
    {
        private static List<string> AllFiles;
        private static List<string> tempPaths;

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
			@"%SYSTEMROOT%\System32\config",
			@"%PROGRAMDATA%\Microsoft\Windows\Start Menu\Programs\Startup",
			@"%SYSTEMROOT%\Prefetch",
			@"%SYSTEMROOT%\Tasks",
			@"%SYSTEMROOT%\SchedLgU.Txt",
			@"%SYSTEMROOT%\System32\winevt\logs",
			@"%SYSTEMROOT%\System32\drivers\etc\hosts",
			@"%SystemDrive%\$MFT"
            };
            defaultPaths = defaultPaths.Select(Environment.ExpandEnvironmentVariables).ToList();

			//This section will attempt to collect files or folder locations under each users profile by pulling their ProfilePath from the registry and adding it in front.
			//Add "defaultPaths.Add($@"{user.ProfilePath}" without the quotes in front of the file / path to be collected in each users profile.
            var users = FindUsers();
            foreach (var user in users)
            {
                //defaultPaths.Add($@"{user.ProfilePath}\NTUSER.DAT");
                //defaultPaths.Add($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat");
            }
            if (Platform.IsUnixLike())
            {
                defaultPaths = new List<string> { };
                tempPaths = new List<string>
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
                    "/etc/group",
                    "/etc/rc.d"
                };
                // Collect file listing
                AllFiles = new List<string> { };
                AllFiles.AddRange(RunCommand("/usr/bin/find", "/ -print"));

                // Find all *.plist files
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("*.plist"))));
                // Find all .bash_history files
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains(".bash_history"))));
                // Find all .sh_history files
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains(".sh_history"))));
                // Find Chrome Preference files
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/History"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Cookies"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Bookmarks"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Extensions"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Last"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Shortcuts"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Top"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("Support/Google/Chrome/Default/Visited"))));

                // Find FireFox Preference Files
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("places.sqlite"))));
                tempPaths.AddRange(AllFiles.Where((stringToCheck => stringToCheck.Contains("downloads.sqlite"))));

                // Fix any spaces to work with MacOS naming conventions
                defaultPaths = tempPaths.ConvertAll(stringToCheck => stringToCheck.Replace(" ", " "));
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

            if (paths.Count == 1)
            {
                if (paths[0] == "")
                {
                    return defaultPaths;
                }
            }
            return paths.Any() ? paths : defaultPaths;
        }
        public static IEnumerable<UserProfile> FindUsers()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList");
            foreach (string name in key.GetSubKeyNames())
            {
                var path = $@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\{name}";
                var profile = Registry.GetValue(path, "FullProfile", string.Empty);
                if (profile != null)
                {
                    var result = new UserProfile
                    {
                        UserKey = name,
                        Path = $@"{path}\ProfileImagePath",
                        ProfilePath = (string)Registry.GetValue(path, "ProfileImagePath", 0),
                        FullProfile = (int)Registry.GetValue(path, "FullProfile", 0)
                    };
                    if (result.FullProfile != -1) yield return result;
                }
            }

        }

        internal class UserProfile
        {
            public string UserKey { get; set; }
            public string Path { get; set; }
            public string ProfilePath { get; set; }
            public int FullProfile { get; set; }
        }
    }
}
