using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using CyLR;
using Xunit;

namespace CyLRTests
{
    public class TestLogger
    {
        [Fact]
        public void TestLoggerLevelConfigs(){
            var logger = new CyLR.Logger();

            // Defaults
            logger.Setup();

            Assert.Equal(CyLR.Logger.Level.trace, logger.fileLevel);
            Assert.Equal(CyLR.Logger.Level.info, logger.consoleLevel);
            logger.TearDown();

            // Warn and Error
            logger.LoggingOptions["output_file_min_level"] = "warn";
            logger.LoggingOptions["output_console_min_level"] = "error";

            logger.Setup();

            Assert.Equal(CyLR.Logger.Level.warn, logger.fileLevel);
            Assert.Equal(CyLR.Logger.Level.error, logger.consoleLevel);
            logger.TearDown();

            // Error and Warn
            logger.LoggingOptions["output_file_min_level"] = "error";
            logger.LoggingOptions["output_console_min_level"] = "warn";

            logger.Setup();

            Assert.Equal(CyLR.Logger.Level.error, logger.fileLevel);
            Assert.Equal(CyLR.Logger.Level.warn, logger.consoleLevel);

            // Test each of the levels for both output formats
            List<CyLR.Logger.Level> levels = new List<CyLR.Logger.Level>{
                CyLR.Logger.Level.trace,
                CyLR.Logger.Level.debug,
                CyLR.Logger.Level.info,
                CyLR.Logger.Level.warn,
                CyLR.Logger.Level.error,
                CyLR.Logger.Level.critical,
                CyLR.Logger.Level.none
            };

            logger.TearDown();

            foreach (var l in levels)
            {
                logger.LoggingOptions["output_file_min_level"] = l.ToString();
                logger.LoggingOptions["output_console_min_level"] = l.ToString();

                logger.Setup();

                Assert.Equal(l, logger.fileLevel);
                Assert.Equal(l, logger.consoleLevel);

                logger.TearDown();
            }
        }

        [Fact]
        public void TestLoggerFormats(){
            var logger = new CyLR.Logger();

            logger.Setup();

            logger.logger(CyLR.Logger.Level.warn, "This is a warning!");
            var expected = "[warn] This is a warning!\n";
            var actual = logger.logMessages.Split(" ", 2)[1];

            Assert.Equal(expected, actual);
            
            logger.TearDown();
        }

        [Fact]
        public void TestLoggerLevelMessages(){
            var logger = new CyLR.Logger();
            string expected;
            string actual;

            logger.Setup();

            logger.trace("This is a trace!");
            expected = "[trace] This is a trace!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.debug("This is a debug!");
            expected = "[debug] This is a debug!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.info("This is a info!");
            expected = "[info] This is a info!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.warn("This is a warning!");
            expected = "[warn] This is a warning!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.error("This is a error!");
            expected = "[error] This is a error!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.critical("This is a critical!");
            expected = "[critical] This is a critical!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);
            logger.logMessages = "";

            logger.none("This is a none!");
            expected = "[none] This is a none!\n";
            actual = logger.logMessages.Split(" ", 2)[1];
            Assert.Equal(expected, actual);

            logger.TearDown();
        }
    }
}