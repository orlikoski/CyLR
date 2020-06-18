using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyLR
{
    public class Arguments
    {
        private const string BaseHelpMessage = "CyLR Version {0}\n\nUsage: {1} [Options]... [Files]...\n\nThe CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.\n\nThe avalable options are:";
        private static readonly Dictionary<string, string> HelpTopics = new Dictionary<string, string>
        {
            {
                "-od",
                "Defines the directory that the zip archive will be created in. Defaults to current working directory.\nUsage: -od <directory path>"
            },
            {
                "-of",
                "Defines the name of the zip archive will be created. Defaults to host machine's name.\nUsage: -of <archive name>"
            },
            {
                "-c",
                "Optional argument to provide custom list of artifact files and directories (one entry per line).\nNOTE: Must use full path including drive letter on each line.  MFT can be collected by \"C:\\$MFT\" or \"D:\\$MFT\" and so on.\nUsage: -c <path to config file>"
            },
            {
                "-u",
                "SFTP username"
            },
            {
                "-p",
                "SFTP password"
            },
            {
                "-s",
                "SFTP Server resolvable hostname or IP address and port. If no port is given then 22 is used by default.  Format is <server name>:<port>\n Usage: -s 8.8.8.8:22"
            },
            {
                "--dry-run",
                "Collect artifacts to a virtual zip archive, but does not send or write to disk."
            },
            {
                "--force-native",
                "Uses the native file system instead of a raw NTFS read. Unix-like environments always use this option."
            },
            {
                "-zp",
                "Uses a password to encrypt the archive file"
            },
            {
                "-zl",
                "Uses a number between 1-9 to change the compression level of the archive file"
            },
            {
                "--no-usnjrnl",
                "Skips collecting $UsnJrnl"
            }
        };

        public readonly bool HelpRequested;

        public readonly string HelpTopic;

        public readonly string CollectionFilePath = ".";
        public readonly List<string> CollectionFiles = null;
        public readonly string OutputPath = ".";
        public readonly string OutputFileName = $"{Environment.MachineName}.zip";
        public readonly bool UseSftp;
        public readonly string UserName = string.Empty;
        public readonly string UserPassword = string.Empty;
        public readonly string SFTPServer = string.Empty;
        public readonly bool DryRun;
        public readonly bool ForceNative;
        public readonly string ZipPassword;
        public readonly string ZipLevel;
        public readonly bool Usnjrnl = true;

        public Arguments(IEnumerable<string> args)
        {
            ForceNative = !Platform.SupportsRawAccess(); //default this to whether or not the platform supports raw access
            var argEnum = args.GetEnumerator();
            while (!HelpRequested && argEnum.MoveNext())
            {
                switch (argEnum.Current)
                {
                    case "--help":
                    case "-h":
                    case "/?":
                    case "--version":
                        HelpRequested = true;
                        argEnum.GetArgumentParameter(ref HelpTopic);
                        break;

                    case "-od":
                        OutputPath = argEnum.GetArgumentParameter();
                        break;

                    case "-of":
                        OutputFileName = argEnum.GetArgumentParameter();
                        break;

                    case "-u":
                        UserName = argEnum.GetArgumentParameter();
                        break;

                    case "-p":
                        UserPassword = argEnum.GetArgumentParameter();
                        break;

                    case "-s":
                        SFTPServer = argEnum.GetArgumentParameter();
                        break;

                    case "-c":
                        CollectionFilePath = argEnum.GetArgumentParameter();
                        break;

                    case "-zp":
                        ZipPassword = argEnum.GetArgumentParameter();
                        break;

                    case "-zl":
                        ZipLevel = argEnum.GetArgumentParameter();
                        break;

                    case "--no-usnjrnl":
                        Usnjrnl = false;
                        break;

                    case "--force-native":
                        if (ForceNative)
                        {
                            Console.WriteLine("Warning: This platform only supports native reads, --force-native has no effect.");
                        }
                        ForceNative = true;
                        break;

                    case "--dry-run":
                        DryRun = true;
                        break;

                    case "-o":
                        throw new ArgumentException("-o is no longer supported, please use -od instead.");

                    default:
                        CollectionFiles = CollectionFiles ?? new List<string>();
                        CollectionFiles.Add(argEnum.Current);
                        break;
                }
            }

            if (!HelpRequested)
            {
                var sftpArgs = new[] { UserName, UserPassword, SFTPServer };
                UseSftp = sftpArgs.Any(arg => !string.IsNullOrEmpty(arg));
                if (UseSftp && sftpArgs.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException("The flags -u, -p, and -s must all have values to continue.  Please try again.");
                }

                if (DryRun)
                {
                    //Disable SFTP in a dry run.
                    UseSftp = false;
                }
            }
        }

        public string GetHelp(string topic)
        {
            string help;
            if (string.IsNullOrEmpty(topic))
            {
                var helpText = new StringBuilder(string.Format(BaseHelpMessage, Version.GetVersion(), AppDomain.CurrentDomain.FriendlyName)).AppendLine();
                foreach (var command in HelpTopics)
                {
                    helpText.AppendLine(command.Key).AppendLine("\t" + command.Value).AppendLine();
                }
                help = helpText.ToString();
            }
            else if (!HelpTopics.TryGetValue(topic, out help))
            {
                help = $@"{topic} is not a valid argument.";
            }
            return help;
        }
    }

    internal static class ArgumentExtentions
    {
        public static string GetArgumentParameter(this IEnumerator<string> arguments)
        {
            var currentArg = arguments.Current;
            var hasArg = arguments.MoveNext();
            if (!hasArg)
            {
                throw new ArgumentException(
                    $"Argument '{currentArg}' had no parameters. Use '--help {currentArg}' for usage details.");
            }

            return arguments.Current;
        }

        public static bool GetArgumentParameter(this IEnumerator<string> arguments, ref string parameter)
        {
            var hasArg = arguments.MoveNext();

            if (hasArg)
            {
                parameter = arguments.Current;
            }

            return hasArg;
        }
    }
}
