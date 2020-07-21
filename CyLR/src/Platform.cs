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

        public static bool IsInputRedirected
        {
            get
            {
                #if DEBUG
                    return false;
                #else
                    return Console.IsInputRedirected;
                #endif
            }
        }
    }
}