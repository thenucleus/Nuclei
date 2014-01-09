//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication
{
    /// <summary>
    /// Stores information about the current connection.
    /// </summary>
    [Serializable]
    internal sealed class CommunicationDescription
    {
        /// <summary>
        /// The version of the communication system that created this instance.
        /// </summary>
        private readonly Version m_Version;

        /// <summary>
        /// The collection of subjects for the communication system.
        /// </summary>
        private readonly List<CommunicationSubject> m_Subjects;

        /// <summary>
        /// The collection containing the types for all the command sets.
        /// </summary>
        private readonly List<ISerializedType> m_Commands;

        /// <summary>
        /// The collection containing the types for all the notification sets.
        /// </summary>
        private readonly List<ISerializedType> m_Notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationDescription"/> class.
        /// </summary>
        /// <param name="communicationVersion">The version of the communication system.</param>
        /// <param name="subjects">The collection of subjects for the current application.</param>
        /// <param name="commands">A collection containing all the known command set types.</param>
        /// <param name="notifications">A collection containing all the known notification set types.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="communicationVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subjects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        public CommunicationDescription(
            Version communicationVersion,
            IEnumerable<CommunicationSubject> subjects,
            IEnumerable<ISerializedType> commands, 
            IEnumerable<ISerializedType> notifications)
        {
            {
                Lokad.Enforce.Argument(() => communicationVersion);
                Lokad.Enforce.Argument(() => subjects);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
            }

            m_Version = communicationVersion;
            m_Subjects = new List<CommunicationSubject>(subjects);
            m_Commands = new List<ISerializedType>(commands);
            m_Notifications = new List<ISerializedType>(notifications);
        }

        /// <summary>
        /// Gets the version of the communication system that created this instance.
        /// </summary>
        public Version CommunicationVersion
        {
            get
            {
                return m_Version;
            }
        }

        /// <summary>
        /// Gets the collection of all available subjects.
        /// </summary>
        public IEnumerable<CommunicationSubject> Subjects
        {
            get
            {
                return m_Subjects;
            }
        }

        /// <summary>
        /// Gets the collection of available commands.
        /// </summary>
        public IEnumerable<ISerializedType> CommandProxies
        {
            get
            {
                return m_Commands;
            }
        }

        /// <summary>
        /// Gets the collection of available notifications.
        /// </summary>
        public IEnumerable<ISerializedType> NotificationProxies
        {
            get
            {
                return m_Notifications;
            }
        }
    }
}
