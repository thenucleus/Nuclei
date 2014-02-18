//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Configuration;

namespace Nuclei.Diagnostics.Logging
{
    /// <summary>
    /// An <see cref="ILogTemplate"/> object that writes to the debug log.
    /// </summary>
    public sealed class DebugLogTemplate : ILogTemplate, IEquatable<DebugLogTemplate>
    {
        /// <summary>
        /// The format string used to format a debug log entry.
        /// </summary>
        public const string DebugLogFormat = @"{0} - {1}: {2}";

        /// <summary>
        /// The configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// A function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_GetCurrentTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogTemplate"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for the application.</param>
        /// <param name="getCurrentTime">A function that gets the current time.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="getCurrentTime"/> is <see langword="null" />.
        /// </exception>
        public DebugLogTemplate(IConfiguration configuration, Func<DateTimeOffset> getCurrentTime)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => getCurrentTime);
            }

            m_Configuration = configuration;
            m_GetCurrentTime = getCurrentTime;
        }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string Name
        {
            [DebuggerStepThrough]
            get
            {
                return "Debug";
            }
        }

        /// <summary>
        /// Returns the default log level which is used if no changes in
        /// log level are requested after the system starts.
        /// </summary>
        /// <returns>
        /// The default log level.
        /// </returns>
        public LevelToLog DefaultLogLevel()
        {
            return m_Configuration.HasValueFor(DiagnosticsConfigurationKeys.DefaultLogLevel)
                ? m_Configuration.Value<LevelToLog>(DiagnosticsConfigurationKeys.DefaultLogLevel)
                : LevelToLog.Error;
        }

        /// <summary>
        /// Translates the specified message.
        /// </summary>
        /// <param name="message">The message that must be translated.</param>
        /// <returns>
        /// The desired string representation of the log message.
        /// </returns>
        public string Translate(ILogMessage message)
        {
            {
                Lokad.Enforce.Argument(() => message);
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                DebugLogFormat,
                m_GetCurrentTime().ToString("yyyy/MM/ddTHH:mm:ss.fffff zzz", CultureInfo.CurrentCulture),
                message.Level,
                message.Text());
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object. </param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(DebugLogTemplate other)
        {
            // All DebugLogTemplate objects are created equal, except for the null objects.
            return other != null;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ILogTemplate other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var template = other as DebugLogTemplate;
            return template != null && Equals(template);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var template = obj as DebugLogTemplate;
            return template != null && Equals(template);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ GetType().GetHashCode();
                hash = (hash * 23) ^ Name.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
