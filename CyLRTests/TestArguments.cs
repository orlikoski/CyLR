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


        [Fact]
        public void TestParseParameterArgs()
        {
            var input_args = new [] {"-s", "127.0.0.1:22", "-u", "username", "-p", "password", "-os", "data",
                "-od", "/sftp", "-of", "host.zip", "-c", "targets.txt", "-zp", "pass", "-zl", "9",
                "mal.exe", "mimi.exe"};
            Arguments args;
            args = new Arguments(input_args);

            Assert.Equal("127.0.0.1:22", args.SFTPServer);
            Assert.Equal("username", args.UserName);
            Assert.Equal("password", args.UserPassword);
            Assert.Equal("data", args.SFTPOutputPath);
            Assert.Equal("/sftp", args.OutputPath);
            Assert.Equal("host.zip", args.OutputFileName);
            Assert.Equal("targets.txt", args.CollectionFilePath);
            Assert.Equal("pass", args.ZipPassword);
            Assert.Equal("9", args.ZipLevel);
            Assert.Contains("mal.exe", args.CollectionFiles);
            Assert.Contains("mimi.exe", args.CollectionFiles);
        }

        [Fact]
        public void TestValidBoolOptions()
        {
            var args = new Arguments(new [] {"--usnjrnl", "--no-sftpcleanup", "--force-native", "--dry-run"});

            Assert.True(args.DryRun);
            Assert.True(args.ForceNative);
            Assert.True(args.Usnjrnl);
            Assert.False(args.SFTPCleanUp);
        }

        [Fact]
        public void TestInvalidParameter()
        {
            Assert.Throws<ArgumentException>(
                () => { new Arguments(new[] {"-s"}); }
                );
        }

        [Fact]
        public void TestValidMetaOptions()
        {
            new Arguments(new [] {"--help", "--version"});
        }

        [Fact]
        public void TestHelpTopic()
        {
            var args = new Arguments(new [] {"-h", "-u"});
            var helpText = args.GetHelp("-u");
            
            Assert.Equal("SFTP username", helpText);
        }

        [Fact]
        public void TestInvalidHelpTopic()
        {
            var args = new Arguments(new [] {"-h", "-o"});
            var helpText = args.GetHelp("-o");
            
            Assert.Equal("-o is not a valid argument.", helpText);
        }
        
        [Fact]
        public void TestHelpTopics()
        {
            var args = new Arguments(new [] {"-h"});
            var helpText = args.GetHelp("");
            
            // Maintaining the exact help language would prove time consuming
            // Instead check that a string of >50 characters is returned.
            Assert.True(helpText.Length > 50);
        }

        [Fact]
        public void TestNoArguments()
        {
            new Arguments(new [] {""});
        }

        [Fact]
        public void TestDepreciatedArgs()
        {
            Assert.Throws<ArgumentException>(
                () => { new Arguments(new [] {"-o"}); }
            );
        }

    }
}