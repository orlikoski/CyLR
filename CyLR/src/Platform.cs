using System;

namespace CyLR
{
    internal static class Platform
    {
        /// <summary>
        /// Is this a unix-like platform?
        /// </summary>
        /// <returns>True if this is a Unix-like platform.</returns>
        public static bool IsUnixLike()
        {
            // Mono reports these numbers as being unix like platforms. Details: http://www.mono-project.com/docs/faq/technical/
            var p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }

        public static bool SupportsRawAccess()
        {
            return !IsUnixLike();
        }

        //http://stackoverflow.com/questions/3453220/how-to-detect-if-console-in-stdin-has-been-redirected :(
        public static bool IsInputRedirected
        {
            get
            {
#if DOT_NET_4_0
                try
                {
                    if (IsUnixLike())
                    {
                        return (0 == (Console.WindowHeight + Console.WindowWidth)); //This works in mono, but not .NET
                    }
                    return ConsoleEx.IsInputRedirected; //this works in .NET, but not mono
                }
                catch (Exception)
                {
                    return true;
                }
#else
                return Console.IsInputRedirected;
#endif
            }
        }
    }
}
