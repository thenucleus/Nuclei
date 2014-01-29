//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Communication.Interaction;

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
        /// The collection containing all the commands for the application.
        /// </summary>
        private readonly List<ISerializedType> m_Commands
            = new List<ISerializedType>();

        /// <summary>
        /// The collection containing all the notifications for the application.
        /// </summary>
        private readonly List<ISerializedType> m_Notifications
            = new List<ISerializedType>();

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
        /// Registers a <see cref="ICommandSet"/> type.
        /// </summary>
        /// <param name="commandType">The <see cref="ICommandSet"/> type.</param>
        public void RegisterCommandType(Type commandType)
        {
            {
                Lokad.Enforce.Argument(() => commandType);
            }

            m_Commands.Add(ProxyExtensions.FromType(commandType));
        }

        /// <summary>
        /// Registers a <see cref="INotificationSet"/> type.
        /// </summary>
        /// <param name="notificationType">The <see cref="INotificationSet"/> type.</param>
        public void RegisterNotificationType(Type notificationType)
        {
            {
                Lokad.Enforce.Argument(() => notificationType);
            }

            m_Notifications.Add(ProxyExtensions.FromType(notificationType));
        }

        /// <summary>
        /// Gets the version of the communication layer used in the application.
        /// </summary>
        public Version CommunicationVersion
        {
            get
            {
                return CommunicationConstants.CommunicationVersion;
            }
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

            return new CommunicationDescription(new List<CommunicationSubject>(m_Subjects),
                new List<ISerializedType>(m_Commands),
                new List<ISerializedType>(m_Notifications));
        }
    }
}
