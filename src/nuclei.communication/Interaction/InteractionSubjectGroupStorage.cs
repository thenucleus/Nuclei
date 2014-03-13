//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about the commands and notifications related to a specific <see cref="CommunicationSubject"/>.
    /// </summary>
    internal sealed class InteractionSubjectGroupStorage : IRegisterSubjectGroups, IStoreInteractionSubjects
    {
        /// <summary>
        /// Stores information about the commands and notifications in a subject group.
        /// </summary>
        private sealed class SubjectMap
        {
            /// <summary>
            /// The collection of commands for a subject group.
            /// </summary>
            private readonly Dictionary<string, SortedList<Version, Type>> m_Commands
                = new Dictionary<string, SortedList<Version, Type>>();

            /// <summary>
            /// The collection of notifications for a subject group.
            /// </summary>
            private readonly Dictionary<string, SortedList<Version, Type>> m_Notifications
                = new Dictionary<string, SortedList<Version, Type>>();

            /// <summary>
            /// Adds a new command to the subject group.
            /// </summary>
            /// <param name="toGroup">The set of command types to which the current command type belongs.</param>
            /// <param name="command">The command type.</param>
            /// <param name="commandVersion">The command version, used for ordering of the commands in a set.</param>
            public void AddCommand(string toGroup, Type command, Version commandVersion)
            {
                {
                    Debug.Assert(toGroup != null, "The group identifier should not be a null reference.");
                    Debug.Assert(command != null, "The command type should not be a null reference.");
                    Debug.Assert(commandVersion != null, "The command version should not be a null reference.");
                }

                if (!m_Commands.ContainsKey(toGroup))
                {
                    m_Commands.Add(toGroup, new SortedList<Version, Type>());
                }

                var list = m_Commands[toGroup];
                list.Add(commandVersion, command);
            }

            /// <summary>
            /// Adds a new notification to the subject group.
            /// </summary>
            /// <param name="toGroup">The set of notification types to which the current notification type belongs.</param>
            /// <param name="notification">The notification type.</param>
            /// <param name="notificationVersion">The notification version, used for ordering of the notification in a set.</param>
            public void AddNotification(string toGroup, Type notification, Version notificationVersion)
            {
                {
                    Debug.Assert(toGroup != null, "The group identifier should not be a null reference.");
                    Debug.Assert(notification != null, "The command type should not be a null reference.");
                    Debug.Assert(notificationVersion != null, "The command version should not be a null reference.");
                }

                if (!m_Notifications.ContainsKey(toGroup))
                {
                    m_Notifications.Add(toGroup, new SortedList<Version, Type>());
                }

                var list = m_Notifications[toGroup];
                list.Add(notificationVersion, notification);
            }

            /// <summary>
            /// Creates a new <see cref="CommunicationSubjectGroup"/>.
            /// </summary>
            /// <returns>The new group.</returns>
            public CommunicationSubjectGroup ToGroup(CommunicationSubject subject)
            {
                var commands = m_Commands
                    .Select(
                        pair => new VersionedTypeFallback(
                            pair.Value
                                .Select(subPair => new Tuple<OfflineTypeInformation, Version>(
                                    new OfflineTypeInformation(subPair.Value.FullName, subPair.Value.Assembly.GetName()), 
                                    subPair.Key))
                                .ToArray()))
                    .ToArray();

                var notifications = m_Notifications
                    .Select(
                        pair => new VersionedTypeFallback(
                            pair.Value
                                .Select(
                                    subPair => new Tuple<OfflineTypeInformation, Version>(
                                        new OfflineTypeInformation(subPair.Value.FullName, subPair.Value.Assembly.GetName()),
                                        subPair.Key))
                                .ToArray()))
                    .ToArray();

                return new CommunicationSubjectGroup(
                    subject,
                    commands,
                    notifications);
            }
        }

        /// <summary>
        /// The collection containing the information about all the commands and notifications that have been
        /// registered by the current application.
        /// </summary>
        private readonly Dictionary<CommunicationSubject, SubjectMap> m_ProvidedSubjects
            = new Dictionary<CommunicationSubject, SubjectMap>();

        /// <summary>
        /// The collection containing the information about all the commands and notifications that are desired
        /// from a remote endpoint.
        /// </summary>
        private readonly Dictionary<CommunicationSubject, SubjectMap> m_RequiredSubjects
            = new Dictionary<CommunicationSubject, SubjectMap>();

        /// <summary>
        /// Registers an existing command with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="version">The version of the command which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of commands that provide similar functionality.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subject"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupIdentifier"/> is <see langword="null" />.
        /// </exception>
        public void RegisterCommandForProvidedSubjectGroup(CommunicationSubject subject, Type commandType, Version version, string groupIdentifier)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => commandType);
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => groupIdentifier);
            }

            if (!m_ProvidedSubjects.ContainsKey(subject))
            {
                m_ProvidedSubjects.Add(subject, new SubjectMap());
            }

            var map = m_ProvidedSubjects[subject];
            map.AddCommand(groupIdentifier, commandType, version);
        }

        /// <summary>
        /// Registers a required command with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="version">The version of the command which is used to order commands that provide similar functionality</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of commands that provide similar functionality.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subject"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupIdentifier"/> is <see langword="null" />.
        /// </exception>
        public void RegisterCommandForRequiredSubjectGroup(CommunicationSubject subject, Type commandType, Version version, string groupIdentifier)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => commandType);
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => groupIdentifier);
            }

            if (!m_RequiredSubjects.ContainsKey(subject))
            {
                m_RequiredSubjects.Add(subject, new SubjectMap());
            }

            var map = m_RequiredSubjects[subject];
            map.AddCommand(groupIdentifier, commandType, version);
        }

        /// <summary>
        /// Registers an existing notification with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <param name="version">The version of the notification which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of notifications that provide similar functionality.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subject"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupIdentifier"/> is <see langword="null" />.
        /// </exception>
        public void RegisterNotificationForProvidedSubjectGroup(CommunicationSubject subject, Type notificationType, Version version, string groupIdentifier)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => notificationType);
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => groupIdentifier);
            }

            if (!m_ProvidedSubjects.ContainsKey(subject))
            {
                m_ProvidedSubjects.Add(subject, new SubjectMap());
            }

            var map = m_ProvidedSubjects[subject];
            map.AddNotification(groupIdentifier, notificationType, version);
        }

        /// <summary>
        /// Registers a required notification with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <param name="version">The version of the notification which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of notifications that provide similar functionality.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subject"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupIdentifier"/> is <see langword="null" />.
        /// </exception>
        public void RegisterNotificationForRequiredSubjectGroup(CommunicationSubject subject, Type notificationType, Version version, string groupIdentifier)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => notificationType);
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => groupIdentifier);
            }

            if (!m_RequiredSubjects.ContainsKey(subject))
            {
                m_RequiredSubjects.Add(subject, new SubjectMap());
            }

            var map = m_RequiredSubjects[subject];
            map.AddNotification(groupIdentifier, notificationType, version);
        }

        /// <summary>
        /// Returns a collection containing all the subjects registered for the current application.
        /// </summary>
        /// <returns>A collection containing all the subjects registered for the current application.</returns>
        public IEnumerable<CommunicationSubject> Subjects()
        {
            return m_ProvidedSubjects.Keys;
        }

        /// <summary>
        /// Creates a new <see cref="ProtocolDescription"/> instance which contains all the 
        /// information about the current state of the communication system.
        /// </summary>
        /// <returns>The new <see cref="ProtocolDescription"/> instance.</returns>
        public ProtocolDescription ToStorage()
        {
            return new ProtocolDescription(m_ProvidedSubjects.Keys);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<CommunicationSubject> GetEnumerator()
        {
            return m_ProvidedSubjects.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a value indicating if a subject group with required commands and notifications exists for 
        /// the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// <see langword="true" /> if a subject group with required commands and notifications exists for the
        /// given subject; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsGroupRequirementsForSubject(CommunicationSubject subject)
        {
            return (subject != null) && m_RequiredSubjects.ContainsKey(subject);
        }

        /// <summary>
        /// Returns the required subject group that is related with the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The required subject group that is related with the given subject.</returns>
        public CommunicationSubjectGroup GroupRequirementsFor(CommunicationSubject subject)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.With<UnknownCommunicationSubjectException>(
                    m_RequiredSubjects.ContainsKey(subject),
                    Resources.Exceptions_Messages_UnknownCommunicationSubject);
            }

            var map = m_RequiredSubjects[subject];
            return map.ToGroup(subject);
        }

        /// <summary>
        /// Returns a value indicating if a subject group with provided commands and notifications exists for 
        /// the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// <see langword="true" /> if a subject group with provided commands and notifications exists for the
        /// given subject; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsGroupProvisionsForSubject(CommunicationSubject subject)
        {
            return (subject != null) && m_ProvidedSubjects.ContainsKey(subject);
        }

        /// <summary>
        /// Returns the provided subject group that is related with the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The provided subject group that is related with the given subject.</returns>
        public CommunicationSubjectGroup GroupProvisionsFor(CommunicationSubject subject)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.With<UnknownCommunicationSubjectException>(
                    m_RequiredSubjects.ContainsKey(subject),
                    Resources.Exceptions_Messages_UnknownCommunicationSubject);
            }

            var map = m_ProvidedSubjects[subject];
            return map.ToGroup(subject);
        }
    }
}
