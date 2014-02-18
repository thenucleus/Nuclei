//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nuclei.Diagnostics.Properties;

namespace Nuclei.Diagnostics.Logging
{
    /// <summary>
    /// Defines a message that should be logged by an <see cref="ILogger"/> object.
    /// </summary>
    [Serializable]
    public sealed class LogMessage : ILogMessage
    {
        /// <summary>
        /// The collection that stores the properties for the message.
        /// </summary>
        private readonly IDictionary<string, object> m_Properties;

        /// <summary>
        /// The text for the log message.
        /// </summary>
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="level">The level of the log message.</param>
        /// <param name="text">The text for the log message.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="level"/> is <see cref="LevelToLog.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="text"/> is <see langword="null" />.
        /// </exception>
        public LogMessage(LevelToLog level, string text)
            : this(level, text, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="level">The level of the log message.</param>
        /// <param name="text">The text for the log message.</param>
        /// <param name="properties">The dictionary that contains the additional properties for the current message.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="level"/> is <see cref="LevelToLog.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="text"/> is <see langword="null" />.
        /// </exception>
        public LogMessage(LevelToLog level, string text, IDictionary<string, object> properties)
        {
            {
                Lokad.Enforce.With<ArgumentException>(level != LevelToLog.None, Resources.Exceptions_Messages_CannotLogMessageWithLogLevelSetToNone);
                Lokad.Enforce.Argument(() => text);
            }

            Level = level;
            m_Text = text;
            m_Properties = properties;
        }

        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        public LevelToLog Level
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the message text for this message.
        /// </summary>
        /// <returns>
        /// The text for this message.
        /// </returns>
        public string Text()
        {
            return m_Text;
        }

        /// <summary>
        /// Gets a value indicating whether the message contains additional parameters that
        /// should be processed when the message is written to the log.
        /// </summary>
        public bool HasAdditionalInformation
        {
            [DebuggerStepThrough]
            get
            {
                return m_Properties != null;
            }
        }

        /// <summary>
        /// Gets a collection that contains additional parameters which should be
        /// processed when the message is written to the log.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Properties
        {
            [DebuggerStepThrough]
            get
            {
                return m_Properties;
            }
        }
    }
}
