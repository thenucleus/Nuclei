//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Diagnostics
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the diagnostics layers.
    /// </summary>
    public static class DiagnosticsConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP port (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey DefaultLogLevel
            = new ConfigurationKey("DefaultLogLevel", typeof(LevelToLog));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the diagnostics section.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the diagnostics section.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    DefaultLogLevel,
                };
        }
    }
}
