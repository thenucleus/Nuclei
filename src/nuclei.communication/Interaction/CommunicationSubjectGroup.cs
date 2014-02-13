//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the communication subject which two endpoints want to communicate about, and the 
    /// ways this communication can be achieved through commands and notifications.
    /// </summary>
    internal sealed class CommunicationSubjectGroup
    {
        /// <summary>
        /// The communication subject that indicates a topic the endpoint will communicate about.
        /// </summary>
        private readonly CommunicationSubject m_Subject;

        /// <summary>
        /// The collection of commands that should be available for the current subject.
        /// </summary>
        private readonly VersionedTypeFallback[] m_Commands;
        
        /// <summary>
        /// The collection of notifications that should be available for the current subject.
        /// </summary>
        private readonly VersionedTypeFallback[] m_Notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSubjectGroup"/> class.
        /// </summary>
        /// <param name="subject">The communication subject that indicates a topic the endpoint will communicate about.</param>
        /// <param name="commands">The collection of all commands that should be available for the current subject.</param>
        /// <param name="notifications">The collection of all notifications that should be available for the current subject.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subject"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        public CommunicationSubjectGroup(CommunicationSubject subject, VersionedTypeFallback[] commands, VersionedTypeFallback[] notifications)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
            }

            m_Subject = subject;
            m_Commands = commands;
            m_Notifications = notifications;
        }

        /// <summary>
        /// Gets the communication subject which indicates a topic the endpoint will communicate about.
        /// </summary>
        public CommunicationSubject Subject
        {
            [DebuggerStepThrough]
            get
            {
                return m_Subject;
            }
        }

        /// <summary>
        /// Gets the collection of commands that should be available for the current subject.
        /// </summary>
        public VersionedTypeFallback[] Commands
        {
            [DebuggerStepThrough]
            get
            {
                return m_Commands;
            }
        }

        /// <summary>
        /// Gets the collection of notifications that should be available for the current subject.
        /// </summary>
        public VersionedTypeFallback[] Notifications
        {
            [DebuggerStepThrough]
            get
            {
                return m_Notifications;
            }
        }
    }
}
