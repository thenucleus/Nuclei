//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Stores the communication subjects, commands and notifications for the application.
    /// </summary>
    internal sealed class CommunicationDescriptionStorage : IStoreCommunicationDescriptions
    {
        /// <summary>
        /// The collection containing all the communication subjects for the application.
        /// </summary>
        private readonly List<CommunicationSubject> m_Subjects
            = new List<CommunicationSubject>();

        /// <summary>
        /// Registers a new subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        public void RegisterApplicationSubject(CommunicationSubject subject)
        {
            {
                Lokad.Enforce.Argument(() => subject);
            }

            m_Subjects.Add(subject);
        }

        /// <summary>
        /// Returns a collection containing all the subjects registered for the current application.
        /// </summary>
        /// <returns>A collection containing all the subjects registered for the current application.</returns>
        public IEnumerable<CommunicationSubject> Subjects()
        {
            return m_Subjects;
        }

        /// <summary>
        /// Creates a new <see cref="CommunicationDescription"/> instance which contains all the 
        /// information about the current state of the communication system.
        /// </summary>
        /// <returns>The new <see cref="CommunicationDescription"/> instance.</returns>
        public CommunicationDescription ToStorage()
        {
            if (m_Subjects.Count == 0)
            {
                throw new NoCommunicationSubjectsRegisteredException();
            }

            return new CommunicationDescription(new List<CommunicationSubject>(m_Subjects));
        }
    }
}
