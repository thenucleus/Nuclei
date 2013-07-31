//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Diagnostics.Logging
{
    /// <summary>
    /// Defines the interface for objects that log information.
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Gets or sets the current <see cref="LevelToLog"/>.
        /// </summary>
        LevelToLog Level
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if a message will be written to the log file based on the
        /// current log level and the level of the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <see langword="true" /> if the message will be logged; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ShouldLog(ILogMessage message);

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(ILogMessage message);

        /// <summary>
        /// Stops the logger and ensures that all log messages have been
        /// saved to the log.
        /// </summary>
        void Close();
    }
}
