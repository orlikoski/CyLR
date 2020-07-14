using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using CyLR;
using Xunit;

namespace CyLRTests
{
    public class TestCollectionPaths
    {
        [Fact]
        public void TestCustomCollectionPaths(){
            Arguments arg = new Arguments(new [] {"-c", "test.txt"});
            List<string> addtlPaths = new List<string>();
            List<string> paths = new List<string>();
            string sep = "/";
            var userProfile = Environment.ExpandEnvironmentVariables("%HOME%");
            var logger = new CyLR.Logger();
            logger.LoggingOptions["output_file_path"] = "1.log";
            logger.Setup();

            // Set platform specific variables.
            if (!Platform.IsUnixLike())
            {
                sep = "\\";
                userProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            }

            // Define expected test files
            paths.Add(userProfile+sep+"CyLR_test.file");
            paths.Add(userProfile+sep+"CyLR_glob_test");
            paths.Add(userProfile+sep+"CyLR_Regex_test");

            // Create test files
            foreach (var path in paths)
            {
                if (! File.Exists(path))
                {
                    File.Create(path);
                }
            }

            // Create custom patterns file.
            using (StreamWriter sr = new StreamWriter("test.txt"))
            {
                sr.WriteLine("glob\t**"+sep+"CyLR_glob_test");
                sr.WriteLine("static\t"+userProfile+sep+"CyLR_test.file");
                if (Platform.IsUnixLike())
                    sr.WriteLine("regex\t^"+userProfile+sep+"CyLR_[A-Za-z]{5}_test$");
                else
                    sr.WriteLine("regex\t^"+userProfile.Replace("\\", "\\\\")+sep+sep+"CyLR_[A-Za-z]{5}_test$");
            }

            var cPaths = CollectionPaths.GetPaths(arg, addtlPaths, false, logger);
            paths.Sort();
            cPaths.Sort();
            Assert.Equal(paths, cPaths);

        }

        [Fact]
        public void TestCustomCollectionPathsRegExFail(){
            Arguments arg = new Arguments(new [] {"-c", "test.txt"});
            List<string> addtlPaths = new List<string>();
            List<string> paths = new List<string>();
            string sep = "/";
            var userProfile = Environment.ExpandEnvironmentVariables("%HOME%");
            var logger = new CyLR.Logger();
            logger.LoggingOptions["output_file_path"] = "2.log";
            logger.Setup();

            if (!Platform.IsUnixLike())
            {
                sep = "\\";
                userProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            }

            paths.Add(userProfile+sep+"CyLR_Regex_test");
            using (StreamWriter sr = new StreamWriter("test.txt"))
            {
                if (Platform.IsUnixLike())
                    sr.WriteLine("regex\t^"+userProfile+sep+"CyLR_[A-Za-z]{10}_test$");
                else
                    sr.WriteLine("regex\t^"+userProfile.Replace("\\", "\\\\")+sep+sep+"CyLR_[A-Za-z]{10}_test$");
            }

            // Create test files
            foreach (var path in paths)
            {
                if (! File.Exists(path))
                {
                    File.Create(path);
                }
            }

            var cPaths = CollectionPaths.GetPaths(arg, addtlPaths, false, logger);
            paths.Sort();
            cPaths.Sort();
            Assert.NotEqual(paths, cPaths);
            Assert.Empty(cPaths);

        }

        [Fact]
        public void TestCustomCollectionPathsForced(){
            Arguments arg = new Arguments(new [] {"-c", "test.txt"});
            List<string> addtlPaths = new List<string>();
            List<string> paths = new List<string>();
            string sep = "/";
            var userProfile = Environment.ExpandEnvironmentVariables("%HOME%");
            var logger = new CyLR.Logger();
            logger.LoggingOptions["output_file_path"] = "3.log";
            logger.Setup();

            if (!Platform.IsUnixLike())
            {
                sep = "\\";
                userProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            }

            paths.Add(userProfile+sep+"not_a_filename___");
            using (StreamWriter sr = new StreamWriter("test.txt"))
            {
                sr.WriteLine("force\t"+userProfile+sep+"not_a_filename___");
            }

            var cPaths = CollectionPaths.GetPaths(arg, addtlPaths, false, logger);
            paths.Sort();
            cPaths.Sort();
            Assert.Equal(paths, cPaths);
        }

        [Fact]
        public void TestCustomCollectionPathsNoFormatType(){
            Arguments arg = new Arguments(new [] {"-c", "test.txt"});
            List<string> addtlPaths = new List<string>();
            List<string> paths = new List<string>();
            string sep = "/";
            var userProfile = Environment.ExpandEnvironmentVariables("%HOME%");
            var logger = new CyLR.Logger();
            logger.LoggingOptions["output_file_path"] = "4.log";
            logger.Setup();

            if (!Platform.IsUnixLike())
            {
                sep = "\\";
                userProfile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            }

            paths.Add(userProfile+sep+"not_a_filename___");
            using (StreamWriter sr = new StreamWriter("test.txt"))
            {
                sr.WriteLine("\t"+userProfile+sep+"not_a_filename___");
                sr.WriteLine("force "+userProfile+sep+"not_a_filename___");
            }

            var cPaths = CollectionPaths.GetPaths(arg, addtlPaths, false, logger);
            paths.Sort();
            cPaths.Sort();
            Assert.NotEqual(paths, cPaths);
            Assert.Empty(cPaths);
        }

    }
}