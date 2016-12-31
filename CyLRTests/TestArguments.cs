using System;
using CyLR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CyLRTests
{
    [TestClass]
    public class TestArguments
    {
        [TestMethod]
        public void TestValidSFTP()
        {
            new Arguments(new []{"-s", "test.server.com", "-u", "username", "-p", "password"});
        }

        [TestMethod]
        public void TestSFTPMissingParams()
        {
            AssertUtil.ShouldThrow<ArgumentException>(
                () => { new Arguments(new[] {"-s", "test.server.com", "-u", "username"}); }
                );
            AssertUtil.ShouldThrow<ArgumentException>(
                () => { new Arguments(new[] { "-s", "test.server.com", "-p", "password" }); }
                );
            AssertUtil.ShouldThrow<ArgumentException>(
                () => { new Arguments(new[] { "-u", "username", "-p", "password" }); }
                );
        }
    }
}
