
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CyLRTests
{
    internal static partial class AssertUtil
    {
        public static void ShouldThrow<ExceptionType>(Action test)
            where ExceptionType : Exception
        {
            bool succeeded = true;
            try
            {
                test();
            }
            catch (ExceptionType)
            {
                succeeded = false;
            }
            Assert.IsFalse(succeeded);
        }
    }
}
