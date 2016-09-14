using System;
using System.Collections.Generic;
using System.Linq;

namespace PythLR
{
    public class Arguments
    {
        private static readonly Dictionary<string, string> HelpTopics = new Dictionary<string, string>
        {
            {string.Empty, "HALP"},
            {
                "-o",
                "Defines the directory that the zip archive will be created in. Defaults to current working directory.\nUsage: -o <directory path>"
            }
        };

        public readonly bool HelpRequested;

        public readonly string HelpTopic;

        public readonly string OutputPath = ".";

        public Arguments(string[] args)
        {
            HelpRequested = args.HasArgument("--help");
            HelpTopic = HelpRequested ? args.GetArgumentParameter(false, "--help") : string.Empty;

            //If help has been requested, parse no more arguments
            if (!HelpRequested)
            {
                if (args.HasArgument("-o"))
                {
                    OutputPath = args.GetArgumentParameter(true, "-o");
                }
            }
        }

        public string GetHelp(string topic)
        {
            string help;
            if (!HelpTopics.TryGetValue(topic, out help))
            {
                help = @"{topic} is not a valid argument.";
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
            string argumentAlias)
        {
            var argEnumerator = arguments.GetEnumerator();
            while (argEnumerator.MoveNext())
            {
                var currentArg = argEnumerator.Current;

                if (currentArg.Equals(argumentAlias))
                {
                    if (argEnumerator.MoveNext())
                    {
                        return argEnumerator.Current;
                    }
                    if (requireArgument)
                    {
                        throw new ArgumentException(
                            $"Argument '{argumentAlias}' had no parameters. Use '--help {argumentAlias}' for usage details.");
                    }
                    return string.Empty;
                }


                if (currentArg.StartsWith(argumentAlias))
                {
                    return currentArg.Substring(argumentAlias.Length);
                }
            }

            throw new ArgumentException($"Argument '{argumentAlias}' was not found.");
        }
    }
}