//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that contains the return value for an <see cref="ICommandSet"/> method invocation.
    /// </summary>
    [Serializable]
    internal sealed class EndpointInteractionInformationMessage : CommunicationMessage
    {
        /// <summary>
        /// The collection containing type information for one or more command sets.
        /// </summary>
        private readonly OfflineTypeInformation[] m_Commands;

        /// <summary>
        /// The collection containing type information for one or more notification sets.
        /// </summary>
        private readonly OfflineTypeInformation[] m_Notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="commands">The collection containing type information for one or more command sets.</param>
        /// <param name="notifications">The collection containing type information for one or more notification sets.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        public EndpointInteractionInformationMessage(
            EndpointId origin, 
            OfflineTypeInformation[] commands, 
            OfflineTypeInformation[] notifications) : 
            base(origin)
        {
            {
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
            }

            m_Commands = commands;
            m_Notifications = notifications;
        }

        /// <summary>
        /// Gets the collection containing type information for one or more command sets.
        /// </summary>
        public OfflineTypeInformation[] Commands
        {
            [DebuggerStepThrough]
            get
            {
                return m_Commands;
            }
        }

        /// <summary>
        /// Gets the collection containing type information for one or more notification sets.
        /// </summary>
        public OfflineTypeInformation[] Notifications
        {
            [DebuggerStepThrough]
            get
            {
                return m_Notifications;
            }
        }
    }
}
