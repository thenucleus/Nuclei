//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Nuclei.Diagnostics.Logging
{
    /// <summary>
    /// Defines a factory that builds <see cref="ILogger"/> objects.
    /// </summary>
    public static class LoggerBuilder
    {
        private static Target BuildFileTarget(string filePath)
        {
            var fileTarget = new FileTarget
                {
                    // Only write the message. The message should contain all the important
                    // information anyway.
                    Layout = "${message}",

                    // Get the file path for the log file.
                    FileName = filePath,

                    // Create the directories if needed
                    CreateDirs = true,

                    // Automatically flush each message to the file
                    AutoFlush = true,

                    // Always close the file so that we don't lose messages
                    // this does make logging slower though.
                    KeepFileOpen = false,

                    // Always append to the file
                    ReplaceFileContentsOnEachWrite = false,

                    // Do not concurrently write to the logger (at least for now)
                    ConcurrentWrites = false,
                };

            return fileTarget;
        }

        private static Target BuildEventLogTarget(string eventLogSource)
        {
            var eventLogTarget = new EventLogTarget
            {
                // Only write the message. The message should contain all the important 
                // information anyway.
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",

                // The source which has been registered to write to the event log.
                Source = eventLogSource,

                // Always write to the application log for now.
                Log = "Application",

                // Define how we move the event id to the logger.
                EventId = string.Format(CultureInfo.InvariantCulture, "${{event-context:item={0}}}", AdditionalLogMessageProperties.EventId),

                // Define how we move the event category to the logger.
                Category = string.Format(CultureInfo.InvariantCulture, "${{event-context:item={0}}}", AdditionalLogMessageProperties.EventCategory),
            };

            return eventLogTarget;
        }

        private static LogFactory BuildLogFactory(string name, LogLevel minimumLevel, Target target)
        {
            var config = new LoggingConfiguration();
            {
                config.AddTarget(name, target);

                // define only one logging rule. We log everything (*) to the this rule starting
                // at log level TRACE and everything goes to the only target
                config.LoggingRules.Add(new LoggingRule("*", minimumLevel, target));
            }

            var result = new LogFactory(config);
            {
                result.GlobalThreshold = LogLevel.Trace;
                result.ThrowExceptions = true;
            }

            result.EnableLogging(); 
            return result;
        }

        /// <summary>
        /// Builds an <see cref="ILogger"/> object that logs information to a given file.
        /// </summary>
        /// <param name="filePath">The file path to which the information gets logged.</param>
        /// <param name="template">The log template that handles the translation from the <see cref="ILogMessage"/> to the log text.</param>
        /// <returns>
        /// The newly created logger that logs information to a given file.
        /// </returns>
        public static ILogger ForFile(string filePath, ILogTemplate template)
        {
            var factory = BuildLogFactory(template.Name, LogLevel.Trace, BuildFileTarget(filePath));
            return new Logger(factory, template);
        }

        /// <summary>
        /// Builds an <see cref="ILogger"/> object that logs information to the event log.
        /// </summary>
        /// <param name="eventLogSource">The event log source name under which the current application is registered.</param>
        /// <param name="template">The log template that handles the translation from the <see cref="ILogMessage"/> to the log text.</param>
        /// <returns>
        /// The newly created logger that logs information to the event log.
        /// </returns>
        public static ILogger ForEventLog(string eventLogSource, ILogTemplate template)
        {
            var factory = BuildLogFactory(template.Name, LogLevel.Warn, BuildEventLogTarget(eventLogSource));
            return new Logger(factory, template);
        }
    }
}
