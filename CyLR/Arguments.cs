using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyLR
{
    public class Arguments
    {
        private const string BaseHelpMessage = "CyLR Version {0}\n\nThe CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.\n\nThe avalable options are:";
        private static readonly Dictionary<string, string> HelpTopics = new Dictionary<string, string>
        {
            {
                "-o",
                "Defines the directory that the zip archive will be created in. Defaults to current working directory.\nUsage: -o <directory path>"
            },
            {
                "-c",
                "Optional argument to provide custom list of artifact files and directories (one entry per line).\nNOTE: Must use full path including drive letter on each line.  MFT can be collected by \"C:$MFT\" or \"D:$MFT\" and so on.\nUsage: -c <path to config file>"
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
            }
        };

        public readonly bool HelpRequested;

        public readonly string HelpTopic;

        public readonly string CollectionFilePath = ".";
        public readonly string OutputPath = ".";
        public readonly bool UseSftp;
        public readonly string UserName = string.Empty;
        public readonly string UserPassword = string.Empty;
        public readonly string SFTPServer = string.Empty;
        public readonly bool DryRun;

        public Arguments(string[] args)
        {
            HelpRequested = args.HasArgument("--help", "-h", "/?");
            HelpTopic = HelpRequested ? args.GetArgumentParameter(false, "--help", "-h", "/?") : string.Empty;

            //If help has been requested, parse no more arguments
            if (!HelpRequested)
            {
                if (args.HasArgument("-o"))
                {
                    OutputPath = args.GetArgumentParameter(true, "-o");
                }

                if (args.HasArgument("-u"))
                {
                    UserName = args.GetArgumentParameter(true, "-u");
                }
                if (args.HasArgument("-p"))
                {
                    UserPassword = args.GetArgumentParameter(true, "-p");
                }
                if (args.HasArgument("-s"))
                {
                    SFTPServer = args.GetArgumentParameter(true, "-s");
                }
                var sftpArgs = new[] { UserName, UserPassword, SFTPServer };
                UseSftp = sftpArgs.Any(arg => !string.IsNullOrEmpty(arg));
                if (UseSftp && sftpArgs.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException("The flags -u, -p, and -s must all have values to continue.  Please try again.");
                }

                if (args.HasArgument("-c"))
                {
                    CollectionFilePath = args.GetArgumentParameter(true, "-c");
                }
                DryRun = args.HasArgument("--dry-run");
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
                var helpText = new StringBuilder(string.Format(BaseHelpMessage, Version.GetVersion())).AppendLine();
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
        public static bool HasArgument(this IEnumerable<string> arguments, params string[] argumentAliases)
        {
            return arguments.Any(arg => argumentAliases.Any(arg.StartsWith));
        }

        public static string GetArgumentParameter(this IEnumerable<string> arguments, bool requireArgument,
            params string[] argumentAliases)
        {
            var argEnumerator = arguments.GetEnumerator();
            while (argEnumerator.MoveNext())
            {
                var currentArg = argEnumerator.Current;

                if (argumentAliases.Contains(currentArg))
                {
                    if (argEnumerator.MoveNext())
                    {
                        return argEnumerator.Current;
                    }
                    if (requireArgument)
                    {
                        throw new ArgumentException(
                            $"Argument '{currentArg}' had no parameters. Use '--help {currentArg}' for usage details.");
                    }
                    return string.Empty;
                }

                var match = argumentAliases.FirstOrDefault(alias => currentArg.StartsWith(alias));
                if (match != null)
                {
                    return currentArg.Substring(match.Length);
                }
            }

            throw new ArgumentException($"Argument '{argumentAliases.First()}' was not found.");
        }
    }
}