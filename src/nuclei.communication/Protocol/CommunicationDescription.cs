//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Stores information about the current connection.
    /// </summary>
    [Serializable]
    internal sealed class CommunicationDescription
    {
        /// <summary>
        /// The collection of subjects for the communication system.
        /// </summary>
        private readonly List<CommunicationSubject> m_Subjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationDescription"/> class.
        /// </summary>
        /// <param name="subjects">The collection of subjects for the current application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subjects"/> is <see langword="null" />.
        /// </exception>
        public CommunicationDescription(IEnumerable<CommunicationSubject> subjects)
        {
            {
                Lokad.Enforce.Argument(() => subjects);
            }

            m_Subjects = new List<CommunicationSubject>(subjects);
        }

        /// <summary>
        /// Gets the collection of all available subjects.
        /// </summary>
        public IEnumerable<CommunicationSubject> Subjects
        {
            [DebuggerStepThrough]
            get
            {
                return m_Subjects;
            }
        }
    }
}
