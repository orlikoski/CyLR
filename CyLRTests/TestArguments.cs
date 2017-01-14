using System;
using CyLR;
using NUnit.Framework;

namespace CyLRTests
{
    [TestFixture]
    public class TestArguments
    {
        [Test]
        public void TestValidSftp()
        {
            Assert.DoesNotThrow(
                ()=>new Arguments(new []{"-s", "test.server.com", "-u", "username", "-p", "password"})
                );
        }

        [Test]
        public void TestSftpMissingParams()
        {
            Assert.Throws<ArgumentException>(
                () => { new Arguments(new[] {"-s", "test.server.com", "-u", "username"}); }
                );
            Assert.Throws<ArgumentException>(
                () => { new Arguments(new[] { "-s", "test.server.com", "-p", "password" }); }
                );
            Assert.Throws<ArgumentException>(
                () => { new Arguments(new[] { "-u", "username", "-p", "password" }); }
                );
        }
    }
}
