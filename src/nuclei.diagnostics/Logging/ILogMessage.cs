//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Diagnostics.Logging
{
    /// <summary>
    /// Defines a log message.
    /// </summary>
    public interface ILogMessage
    {
        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        LevelToLog Level
        {
            get;
        }

        /// <summary>
        /// Returns the message text for this message.
        /// </summary>
        /// <returns>
        /// The text for this message.
        /// </returns>
        string Text();

        /// <summary>
        /// Gets a value indicating whether the message contains additional parameters that
        /// should be processed when the message is written to the log.
        /// </summary>
        bool HasAdditionalInformation
        {
            get;
        }

        /// <summary>
        /// Gets a collection that contains additional parameters which should be
        /// processed when the message is written to the log.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Properties
        {
            get;
        }
    }
}
