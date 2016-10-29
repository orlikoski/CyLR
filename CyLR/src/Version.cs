using System.Reflection;
using SystemVersion = System.Version;

namespace CyLR
{
    internal static class Version
    {
        public static SystemVersion GetVersion()
        {
            var assem = Assembly.GetEntryAssembly();
            var assemName = assem.GetName();
            return assemName.Version;
        }
    }
}