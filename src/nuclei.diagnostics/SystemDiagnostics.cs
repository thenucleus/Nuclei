//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Nuclei.Diagnostics
{
    /// <summary>
    /// Provides methods that help with diagnosing possible issues with the framework.
    /// </summary>
    public sealed class SystemDiagnostics
    {
        /// <summary>
        /// The profiler that is used to time the different actions in the application.
        /// </summary>
        private readonly Profiler m_Profiler;

        /// <summary>
        /// The action that logs the given string to the underlying loggers.
        /// </summary>
        private readonly Action<LevelToLog, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDiagnostics"/> class.
        /// </summary>
        /// <param name="logger">The action that logs the given string to the underlying loggers.</param>
        /// <param name="profiler">The object that provides interval measuring methods. May be <see langword="null" />.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public SystemDiagnostics(Action<LevelToLog, string> logger, Profiler profiler)
        {
            {
                Lokad.Enforce.Argument(() => logger);
            }

            m_Logger = logger;
            m_Profiler = profiler;
        }

        /// <summary>
        /// Passes the given message to the system loggers.
        /// </summary>
        /// <param name="severity">The severity for the message.</param>
        /// <param name="message">The message.</param>
        public void Log(LevelToLog severity, string message)
        {
            m_Logger(severity, message);
        }

        /// <summary>
        /// Passes the given message to the system loggers.
        /// </summary>
        /// <param name="severity">The severity for the message.</param>
        /// <param name="prefix">The prefix text that should be placed at the start of the logged text.</param>
        /// <param name="message">The message.</param>
        public void Log(LevelToLog severity, string prefix, string message)
        {
            m_Logger(
                severity,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} - {1}",
                    prefix,
                    message));
        }

        /// <summary>
        /// Gets the profiler that can be used to gather timing intervals for any specific action
        /// that is executed in the framework.
        /// </summary>
        public Profiler Profiler
        {
            [DebuggerStepThrough]
            get
            {
                return m_Profiler;
            }
        }
    }
}
