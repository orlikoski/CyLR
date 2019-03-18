using System;
using CyLR;
using Xunit;

namespace CyLRTests
{
    public class TestArguments
    {
        [Fact]
        public void TestValidSftp()
        {
            new Arguments(new[] { "-s", "test.server.com", "-u", "username", "-p", "password" });
        }

        [Fact]
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