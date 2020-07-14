using System;
using System.IO;
using System.Collections.Generic;
using static System.FormattableString;

namespace CyLR
{
    /// <summary>
    /// Class to handle logging operations.
    /// </summary>
    internal class Logger
    {
        /// <summary>Definition of logging levels for filtering.</summary>
        public enum Level
        {
            /// <summary>Level 0, trace events. Most verbosity.</summary>
            trace,
            /// <summary>Level 1, debug events. Shown in console when user increases verbosity.</summary>
            debug,
            /// <summary>Level 2, info events. Notifications to the user not related to concerning activity.</summary>
            info,
            /// <summary>Level 3, warn events. Concerning activity to identify to the user.</summary>
            warn,
            /// <summary>Level 4, error events. Difficulties the use needs to be aware of, possibly leading to incomplete execution.</summary>
            error,
            /// <summary>Level 5, critical events. Messages urgently needed for user consideration.</summary>
            critical,
            /// <summary>Level 6, none events. Used to suppress log messages.</summary>
            none
        }

        /// <summary>String buffer holding log messages.</summary>
        public string logMessages = "";
        /// <summary>Format to write log messages.
        /// Does not support custom format specification at this time.
        /// Format is <c>$DATE [$LEVEL] $MESSAGE</c>
        /// </summary>
        public string msgFormat = "{0} [{1}] {2}";

        /// <summary>File to write log messages to.</summary>
        StreamWriter logfile;
        /// <summary>Default level for messages written to a log file.</summary>
        public Level fileLevel = Level.trace;
        /// <summary>Default level for messages written to the console.</summary>
        public Level consoleLevel = Level.info;

        /// <summary>Main configuration storage</summary>
        public Dictionary<string, string> LoggingOptions = new Dictionary<string, string>
        {
            // Output to file options
            {"output_file_enabled", "true"},
            {"output_file_path", ""},
            {"output_file_min_level", "trace"},

            // Output to console options
            {"output_console_enabled", "true"},
            {"output_console_min_level", "info"},

            // Output to cumulative buffer
            {"output_buffer_enabled", "true"},

            // Formatting options (applies to all outputs)
            {"format_include_timestamp", "true"},
            {"format_include_level", "true"},
            {"format_include_message", "true"},
        };

        /// <summary>Performs set up operations related to the logger</summary>
        /// <remarks>Relies on <c>LoggingOptions</c></remarks>
        public void Setup()
        {
            if (LoggingOptions["output_file_enabled"] == "true")
            {
                // Open log file
                if (LoggingOptions["output_file_path"] == "")
                {
                    logfile = new StreamWriter("CyLR.log", true);
                }
                else
                {
                    logfile = new StreamWriter(LoggingOptions["output_file_path"], true);
                }
            }

            // Set minimum level for outputs
            switch (LoggingOptions["output_file_min_level"])
            {
                case "trace":
                    fileLevel = Level.trace;
                    break;
                case "debug":
                    fileLevel = Level.debug;
                    break;
                case "info":
                    fileLevel = Level.info;
                    break;
                case "warn":
                    fileLevel = Level.warn;
                    break;
                case "error":
                    fileLevel = Level.error;
                    break;
                case "critical":
                    fileLevel = Level.critical;
                    break;
                case "none":
                    fileLevel = Level.none;
                    break;
                default:
                    break;
            }

            switch (LoggingOptions["output_console_min_level"])
            {
                case "trace":
                    consoleLevel = Level.trace;
                    break;
                case "debug":
                    consoleLevel = Level.debug;
                    break;
                case "info":
                    consoleLevel = Level.info;
                    break;
                case "warn":
                    consoleLevel = Level.warn;
                    break;
                case "error":
                    consoleLevel = Level.error;
                    break;
                case "critical":
                    consoleLevel = Level.critical;
                    break;
                case "none":
                    consoleLevel = Level.none;
                    break;
                default:
                    break;
            }
        }

        /// <summary>Performs tear down operations related to the logger</summary>
        public void TearDown()
        {
            logfile.Close();
        }

        /// <summary>Log message writer</summary>
        /// <remark>Will output to destination as specified in <c>LoggingOptions</c></remark>
        /// <param name="level">The <c>Level</c> of the message</param>
        /// <param name="message">The message content to log</param>
        public void logger(Level level, string message)
        {
            var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            var entry = String.Format(msgFormat, now, level.ToString(), message);

            // Logs the message and level based on the configuration
            //
            // Logs to file, if enabled
            if (LoggingOptions["output_file_enabled"] == "true")
            {
                if (fileLevel <= level)
                {
                    logfile.WriteLine(entry);      
                    logfile.Flush();              
                }
            }
            
            // Logs to console
            if (LoggingOptions["output_console_enabled"] == "true")
            {
                if (consoleLevel <= level)
                {
                    Console.WriteLine(entry);                    
                }
            }

            // Append to cumulative log list.
            if (LoggingOptions["output_buffer_enabled"] == "true")
            {
                logMessages += entry + "\n";
            }
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.trace</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void trace(string message)
        {
            logger(Level.trace, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.debug</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void debug(string message)
        {
            logger(Level.debug, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.info</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void info(string message)
        {
            logger(Level.info, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.warn</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void warn(string message)
        {
            logger(Level.warn, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.error</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void error(string message)
        {
            logger(Level.error, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.critical</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void critical(string message)
        {
            logger(Level.critical, message);
        }

        /// <summary>Abstract of <see cref="logger(Level, string)"/> that writes messages as <c>Level.none</c>.</summary>
        /// <param name="message">Message to log.</param>
        public void none(string message)
        {
            logger(Level.none, message);
        }
        
    }
}