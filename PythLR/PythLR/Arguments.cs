using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythLR
{
    static class Arguments
    {
        public static bool HasArgument(this IEnumerable<string> arguments, string argumentSymbol)
        {
            return arguments.Any(arg => arg.StartsWith(argumentSymbol));
        }

        public static string GetArgumentParameter(this IEnumerable<string> arguments, string argumentSymbol)
        {
            var argEnumerator = arguments.GetEnumerator();
            while (argEnumerator.MoveNext())
            {
                var currentArg = argEnumerator.Current;

                if (currentArg.Equals(argumentSymbol))
                {
                    if (!argEnumerator.MoveNext())
                    {
                        throw new ArgumentException($"Argument '{argumentSymbol}' had no parameters");
                    }
                    return argEnumerator.Current;
                }

                if (currentArg.StartsWith(argumentSymbol))
                {
                    return currentArg.Substring(argumentSymbol.Length);
                }
            }
            throw new ArgumentException($"Argument '{argumentSymbol}' was not found.");
        }
    }
}
