using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Win32;
using DotNet.Globbing;
using System.Text.RegularExpressions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CyLRTests")]
namespace CyLR
{
    /// <summary>
    /// Class to handle functionality around file system scanning, pattern
    /// compilation, and providing targets to attempt collection.
    /// </summary>
    internal static class CollectionPaths
    {

        /// <summary>
        /// Method used to apply default and user specified patterns to files
        /// identified on the system.
        ///
        /// All paths and patterns are case insensitive. 
        /// </summary>
        /// <param name="arguments">User arguments provided at execution.</param>
        /// <param name="additionalPaths">Additional collection targets from the command line.</param>
        /// <param name="Usnjrnl">Whether or not to collect the $J.</param>
        /// <param name="logger">A logging object.</param>
        /// <returns>
        /// List of distinct files to attempt collection of from a system. 
        /// This list is filtered by the default and custom patterns.
        /// </returns>
        public static List<string> GetPaths(Arguments arguments, List<string> additionalPaths, bool Usnjrnl, Logger logger)
        {
            // Init with additional paths provided as a parameter
            // Only supports static paths.
            var staticPaths = new List<string>(additionalPaths);

            // Init vars for glob, regex, and paths to collect
            var globPaths = new List<Glob>();
            var regexPaths = new List<Regex>();
            var collectionPaths = new List<string>();

            // Enable case insensitivity
            GlobOptions.Default.Evaluation.CaseInsensitive = true; 
            bool staticCaseInsensitive = true;

            // Init base paths to scan for files and folders
            var basePaths = new List<string>();

            // Get listing of drives to scan based on platform
            if (Platform.IsUnixLike())
            {
                basePaths.Add("/");  // Scan the entire root.
            } 
            else 
            {
                logger.debug("Enumerating volumes on system");
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    basePaths.Add(d.Name.ToString());
                }
                logger.debug(String.Format("Identified volumes: {0}", String.Join(", ", basePaths)));
            }


            // Load information from the CollectionFilePath if present and availble
            if (arguments.CollectionFilePath != ".")
            {
                if (File.Exists(arguments.CollectionFilePath))
                {
                    logger.debug("Extracting patterns from custom path file");
                    using (StreamReader sr = new StreamReader(arguments.CollectionFilePath)){
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            // Skip lines starting with comment
                            if (line.StartsWith("#"))
                            {
                                continue;
                            }

                            // Skip blank lines
                            if (line.Length == 0)
                            {
                                continue;
                            }

                            // Skip paths without tab separator and report to user
                            if (! line.Contains("\t")){
                                logger.warn(String.Format("Excluding invalid path format \"{0}\"", line));
                                continue;
                            }

                            // Split into config components. Requires a definition and path, delimited by a tab
                            string[] pathParts = line.Split('\t');

                            var pathDef = pathParts[0].ToLower();
                            var pathData = Environment.ExpandEnvironmentVariables(pathParts[1]);

                            // Append the path to the proper list based on the definition
                            switch (pathDef)
                            {
                                case "static":
                                    staticPaths.Add(pathData);
                                    break;
                                case "glob":
                                    globPaths.Add(Glob.Parse(pathData));
                                    break;
                                case "regex":
                                    regexPaths.Add(new Regex(pathData));
                                    break;
                                case "force":
                                    collectionPaths.Add(pathData);
                                    break;
                                default:
                                    logger.warn(String.Format("Excluding invalid path format \"{0}\"", line));
                                    break;
                            }
                        }
                    }
                }
                // Handle conditions where the file is not present.
                else
                {
                    logger.error(String.Format("Error: Could not find file: {0}",  arguments.CollectionFilePath));
                    logger.error("Exiting");
                    logger.TearDown();
                    throw new ArgumentException();
                }
            }

            // Load information provided at the command line as additional paths
            if (arguments.CollectionFiles != null)
            {
                logger.debug("Adding command line specified files");
                staticPaths.AddRange(arguments.CollectionFiles);
            }

            bool hasMacOSFolders = (Directory.Exists("/private") 
                && Directory.Exists("/Applications")
                && Directory.Exists("/Users"));

            if (arguments.CollectionFilePath == "." || arguments.CollectDefaults)
            {
                logger.debug("Enumerating patterns for default artifact collection");
                //This section will attempt to collect files or folder locations under each users profile by pulling their ProfilePath from the registry and adding it in front.
                //Add "defaultPaths.Add($@"{user.ProfilePath}" without the quotes in front of the file / path to be collected in each users profile.
                if (!Platform.IsUnixLike())
                {
                    logger.info("Windows platform detected");
                    // Define default paths
                    string systemRoot = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%");
                    string programData = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%");
                    string systemDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
                    globPaths.Add(Glob.Parse(systemRoot + @"\Tasks\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\Prefetch\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\System32\sru\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\System32\winevt\Logs\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\System32\Tasks\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\System32\LogFiles\W3SVC1\**"));
                    globPaths.Add(Glob.Parse(systemRoot + @"\Appcompat\Programs\**"));
                    globPaths.Add(Glob.Parse(programData + @"\Microsoft\Windows\Start Menu\Programs\Startup\**"));
                    globPaths.Add(Glob.Parse(systemDrive + @"\$Recycle.Bin\**\$I*"));
                    globPaths.Add(Glob.Parse(systemDrive + @"\$Recycle.Bin\$I*"));
                    
                    staticPaths.Add(@"%SYSTEMROOT%\SchedLgU.Txt");
                    staticPaths.Add(@"%SYSTEMROOT%\inf\setupapi.dev.log");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\drivers\etc\hosts");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SAM");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SYSTEM");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SOFTWARE");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SECURITY");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SAM.LOG1");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SYSTEM.LOG1");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SOFTWARE.LOG1");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SECURITY.LOG1");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SAM.LOG2");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SYSTEM.LOG2");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SOFTWARE.LOG2");
                    staticPaths.Add(@"%SYSTEMROOT%\System32\config\SECURITY.LOG2");


                    // Send static filesystem artifacts to collectionPaths directly
                    collectionPaths.Add(@"%SystemDrive%\$LogFile");
                    collectionPaths.Add(@"%SystemDrive%\$MFT");
                    // Add USN if enabled
                    if (Usnjrnl)
                    {
                        collectionPaths.Add(@"%SystemDrive%\$Extend\$UsnJrnl:$J");
                    }
                    
                    // Expand envars for all staticPaths.
                    staticPaths = staticPaths.Select(Environment.ExpandEnvironmentVariables).ToList();
                    collectionPaths = collectionPaths.Select(Environment.ExpandEnvironmentVariables).ToList();
                    
                    // Add user specific paths to static list.
                    var users = FindUsers();
                    foreach (var user in users)
                    {
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Roaming\Microsoft\Windows\Recent\**"));
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\WebCache\**"));
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Roaming\Microsoft\Windows\Recent\AutomaticDestinations\**"));
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Roaming\Mozilla\Firefox\Profiles\**"));
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Local\ConnectedDevicesPlatform\**"));
                        globPaths.Add(Glob.Parse($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\Explorer\**"));

                        staticPaths.Add($@"{user.ProfilePath}\NTUSER.DAT");
                        staticPaths.Add($@"{user.ProfilePath}\NTUSER.DAT.LOG1");
                        staticPaths.Add($@"{user.ProfilePath}\NTUSER.DAT.LOG2");
                        staticPaths.Add($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat");
                        staticPaths.Add($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG1");
                        staticPaths.Add($@"{user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG2");
                        staticPaths.Add($@"{user.ProfilePath}\AppData\Local\Google\Chrome\User Data\Default\History");
                        staticPaths.Add($@"{user.ProfilePath}\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt");
                    }
                }
                // Handle macOS platforms
                else if (Platform.IsUnixLike() && hasMacOSFolders)
                {
                    logger.info("macOS platform detected");
                    // Define default paths to collect
                    var defaultPaths = new List<string> 
                    {
                        "/etc/hosts.allow",
                        "/etc/hosts.deny",
                        "/etc/hosts",
                        "/private/etc/hosts.allow",
                        "/private/etc/hosts.deny",
                        "/private/etc/hosts",
                        "/etc/passwd",
                        "/etc/group",
                        "/private/etc/passwd",
                        "/private/etc/group",
                    };
                    staticPaths.AddRange(defaultPaths);

                    // Expand envars for all staticPaths.
                    staticPaths = staticPaths.Select(Environment.ExpandEnvironmentVariables).ToList();


                    var defaultGlobs = new List<Glob> {
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/History*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Cookies*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Bookmarks*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Extensions/**"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Last*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Shortcuts*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Top*"),
                        Glob.Parse("**/Library/*Support/Google/Chrome/Default/Visited*"),
                        Glob.Parse("**/places.sqlite*"),
                        Glob.Parse("**/downloads.sqlite*"),
                        Glob.Parse("**/*.plist"),
                        Glob.Parse("/Users/*/.*history"),
                        Glob.Parse("/root/.*history"),
                        Glob.Parse("/System/Library/StartupItems/**"),
                        Glob.Parse("/System/Library/LaunchAgents/**"),
                        Glob.Parse("/System/Library/LaunchDaemons/**"),
                        Glob.Parse("/Library/LaunchAgents/**"),
                        Glob.Parse("/Library/LaunchDaemons/**"),
                        Glob.Parse("/Library/StartupItems/**"),
                        Glob.Parse("/var/log/**"),
                        Glob.Parse("/private/var/log/**"),
                        Glob.Parse("/private/etc/rc.d/**"),
                        Glob.Parse("/etc/rc.d/**"),
                        Glob.Parse("/.fseventsd/**")
                    };
                    globPaths.AddRange(defaultGlobs);
                    
                } 
                // Handle Linux platforms
                else if (Platform.IsUnixLike())
                {
                    logger.info("Linux platform detected");

                    // Define default paths to collect
                    var defaultPaths = new List<string> 
                    {
                        // Super user
                        "/root/.ssh/config",
                        "/root/.ssh/known_hosts",
                        "/root/.ssh/authorized_keys",
                        "/root/.selected_editor",
                        "/root/.viminfo",
                        "/root/.lesshist",
                        "/root/.profile",
                        "/root/.selected_editor",

                        // Boot
                        "/boot/grub/grub.cfg",
                        "/boot/grub2/grub.cfg",

                        // Sys
                        "/sys/firmware/acpi/tables/DSDT",

                        //etc
                        "/etc/hosts.allow",
                        "/etc/hosts.deny",
                        "/etc/hosts",
                        "/etc/passwd",
                        "/etc/group",
                        "/etc/crontab",
                        "/etc/cron.allow",
                        "/etc/cron.deny",
                        "/etc/anacrontab",
                        "/var/spool/anacron/cron.daily",
                        "/var/spool/anacron/cron.hourly",
                        "/var/spool/anacron/cron.weekly",
                        "/var/spool/anacron/cron.monthly",
                        "/etc/apt/sources.list",
                        "/etc/apt/trusted.gpg",
                        "/etc/apt/trustdb.gpg",
                        "/etc/resolv.conf",
                        "/etc/fstab",
                        "/etc/issues",
                        "/etc/issues.net",
                        "/etc/insserv.conf",
                        "/etc/localtime",
                        "/etc/timezone",
                        "/etc/pam.conf",
                        "/etc/rsyslog.conf",
                        "/etc/xinetd.conf",
                        "/etc/netgroup",
                        "/etc/nsswitch.conf",
                        "/etc/ntp.conf",
                        "/etc/yum.conf",
                        "/etc/chrony.conf",
                        "/etc/chrony",
                        "/etc/sudoers",
                        "/etc/logrotate.conf",
                        "/etc/environment",
                        "/etc/hostname",
                        "/etc/host.conf",
                        "/etc/fstab",
                        "/etc/machine-id",
                        "/etc/screen-rc",
                    };
                    staticPaths.AddRange(defaultPaths);

                    // Expand envars for all staticPaths.
                    staticPaths = staticPaths.Select(Environment.ExpandEnvironmentVariables).ToList();

                    var defaultGlobs = new List<Glob> {
                        // User profiles
                        Glob.Parse("/home/*/.*history"),
                        Glob.Parse("/home/*/.ssh/known_hosts"),
                        Glob.Parse("/home/*/.ssh/config"),
                        Glob.Parse("/home/*/.ssh/autorized_keys"),
                        Glob.Parse("/home/*/.viminfo"),
                        Glob.Parse("/home/*/.profile"),
                        Glob.Parse("/home/*/.*rc"),
                        Glob.Parse("/home/*/.*_logout"),
                        Glob.Parse("/home/*/.selected_editor"),
                        Glob.Parse("/home/*/.wget-hsts"),
                        Glob.Parse("/home/*/.gitconfig"),

                        // Firefox artifacts
                        Glob.Parse("/home/*/.mozilla/firefox/*.default*/**/*.sqlite*"),
                        Glob.Parse("/home/*/.mozilla/firefox/*.default*/**/*.json"),
                        Glob.Parse("/home/*/.mozilla/firefox/*.default*/**/*.txt"),
                        Glob.Parse("/home/*/.mozilla/firefox/*.default*/**/*.db*"),

                        // Chrome artifacts
                        Glob.Parse("/home/*/.config/google-chrome/Default/History*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Cookies*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Bookmarks*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Extensions/**"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Last*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Shortcuts*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Top*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Visited*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Preferences*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Login Data*"),
                        Glob.Parse("/home/*/.config/google-chrome/Default/Web Data*"),

                        // Superuser profiles
                        Glob.Parse("/root/.*history"),
                        Glob.Parse("/root/.*rc"),
                        Glob.Parse("/root/.*_logout"),
                        
                        // var
                        Glob.Parse("/var/log/**"),
                        Glob.Parse("/var/spool/at/**"),
                        Glob.Parse("/var/spool/cron/**"),
                        
                        // etc
                        Glob.Parse("/etc/rc.d/**"),
                        Glob.Parse("/etc/cron.daily/**"),
                        Glob.Parse("/etc/cron.hourly/**"),
                        Glob.Parse("/etc/cron.weekly/**"),
                        Glob.Parse("/etc/cron.monthly/**"),
                        Glob.Parse("/etc/modprobe.d/**"),
                        Glob.Parse("/etc/modprobe-load.d/**"),
                        Glob.Parse("/etc/*-release"),
                        Glob.Parse("/etc/pam.d/**"),
                        Glob.Parse("/etc/rsyslog.d/**"),
                        Glob.Parse("/etc/yum.repos.d/**"),
                        Glob.Parse("/etc/init.d/**"),
                        Glob.Parse("/etc/systemd.d/**"),
                        Glob.Parse("/etc/default/**"),

                    };
                    globPaths.AddRange(defaultGlobs);
                    
                } 
                else 
                {
                    logger.error("Unsupported platform");
                    logger.TearDown();
                    throw new Exception();
                }
            }

            // Perform case operations
            if (staticCaseInsensitive)
            {
                staticPaths = staticPaths.Select(x => x.ToLower()).ToList();
            }


            // Get file system listing to populate collection paths
            logger.debug("Enumerating file systems and matching patterns");
            var num_paths_scanned = 0;
            foreach (var basePath in basePaths)
            {
                logger.debug(String.Format("Enumerating volume: {0}", basePath));
                foreach (var entry in WalkTree(basePath, logger))
                {
                    num_paths_scanned++;
                    // Convert to string for ease of comparison
                    var entryStr = entry.ToString();
                    string staticEntry = entryStr;

                    if (staticCaseInsensitive)
                    {
                        staticEntry = entryStr.ToLower();
                    } 

                    // If found in the staticPaths list, add to the collection
                    if (staticPaths.Contains(staticEntry)){
                        collectionPaths.Add(entryStr);
                        continue;
                    }

                    // If not found in the static list, evaluate glob first
                    // as it is more efficient than regex
                    bool globFound = false;
                    foreach (var globPattern in globPaths)
                    {
                        try
                        {
                            globFound = globPattern.IsMatch(entryStr);
                        }
                        catch (System.Exception)
                        {
                            logger.error("Unknown globbing error encountered. Please report.");
                            throw;
                        }
                        if (globFound)
                        {
                            collectionPaths.Add(entryStr);
                            break; 
                        }
                    }

                    if (globFound)
                        continue;

                    // Lastly evaluate regex
                    bool regexFound = false;
                    foreach (var regexPattern in regexPaths)
                    {
                        try
                        {
                            regexFound = regexPattern.IsMatch(entryStr);
                        }
                        catch (System.Exception)
                        {
                            logger.error("Unknown regex error encountered. Please report.");
                            throw;
                        }
                        if (regexFound)
                        {
                            collectionPaths.Add(entryStr);
                            break;
                        }
                    }
                    
                    if (regexFound)
                    {
                        continue;
                    }
                }
            }


            // Remove empty strings from custom paths
            if (collectionPaths.Any()){
                collectionPaths.RemoveAll(x => string.IsNullOrEmpty(x));
            }
            logger.info(String.Format("Scanned {0} paths", num_paths_scanned));
            logger.info(String.Format("Found {0} paths to collect", collectionPaths.Count));

            // Return paths to collect
            return collectionPaths;
        }

        /// <summary>
        /// Method used to enumerate files recursively from a location on a drive.
        /// </summary>
        /// <param name="basePath">A string value containing the root to walk recursively.</param>
        /// <param name="logger">A logging object.</param>
        /// <returns>
        /// Yields an <c>IEnumerable</c> containing <c>FileInfo</c> records for each
        /// file found within the path.
        /// </returns>
        private static IEnumerable<FileInfo> WalkTree(string basePath, Logger logger)
        {
            var dirStack = new Stack<DirectoryInfo>();
            dirStack.Push(new DirectoryInfo(basePath));

            while (dirStack.Count > 0)
            {
                var dir = dirStack.Pop();

                // Get sub directories to add to stack
                // handle access issues
                try
                {
                    foreach (var subDir in dir.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.ReparsePoint)))
                    {
                        dirStack.Push(subDir);
                    }
                }
                catch (System.Exception)
                {
                    logger.warn(String.Format("Unable to enumerate all files and sub directories in {0}", dir.ToString()));
                }

                // Get files within current directory
                // Handle file access exceptions.
                List<FileInfo> allFiles = new List<FileInfo>();
                try
                {
                    foreach (var fileEntry in dir.GetFiles())
                    {   
                        allFiles.Add(fileEntry);
                    }   
                }
                catch (System.IO.IOException){
                    logger.warn(String.Format("Cannot read one or more files in {0}", dir.ToString()));
                }
                catch (System.UnauthorizedAccessException){
                    logger.warn(String.Format("Access is denied to one or more files in {0}", dir.ToString()));
                }
                catch (System.Exception)
                {
                    logger.warn(String.Format("Unable to enumerate one or more files in {0}", dir.ToString()));
                }

                foreach (var f in allFiles)
                {
                    yield return f;
                }
            }   
        }

        /// <summary>
        /// Method to enumerate user profiles on a Windows system.
        /// </summary>
        /// <returns>
        /// Yields an <c>IEnumerable</c> containing <c>UserProfile</c> records for each
        /// account identified on the system.
        /// </returns>
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