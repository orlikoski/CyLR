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
            var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            var expected = $"{now} [warn] This is a warning!\n";

            Assert.Equal(expected, logger.logMessages);
            
            logger.TearDown();
        }

        [Fact]
        public void TestLoggerLevelMessages(){
            var logger = new CyLR.Logger();
            var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string expected;

            logger.Setup();

            logger.trace("This is a trace!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [trace] This is a trace!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.debug("This is a debug!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [debug] This is a debug!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.info("This is a info!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [info] This is a info!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.warn("This is a warning!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [warn] This is a warning!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.error("This is a error!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [error] This is a error!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.critical("This is a critical!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [critical] This is a critical!\n";
            Assert.Equal(expected, logger.logMessages);
            logger.logMessages = "";

            logger.none("This is a none!");
            now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            expected = $"{now} [none] This is a none!\n";
            Assert.Equal(expected, logger.logMessages);

            logger.TearDown();
        }
    }
}