//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that indicates whether the sending endpoint wants to communicate with the
    /// receiving endpoint.
    /// </summary>
    internal sealed class EndpointInteractionInformationResponseMessage : CommunicationMessage
    {
        /// <summary>
        /// A value that indicates if the sending endpoint is interested in maintaining the
        /// connection or not.
        /// </summary>
        private readonly InteractionConnectionState m_State;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInteractionInformationResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <param name="state">The value that indicates if the sending endpoint is interested in maintaining the connection or not.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public EndpointInteractionInformationResponseMessage(
            EndpointId origin,
            MessageId inResponseTo,
            InteractionConnectionState state)
            : this(origin, new MessageId(), inResponseTo, state)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInteractionInformationResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <param name="state">The value that indicates if the sending endpoint is interested in maintaining the connection or not.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public EndpointInteractionInformationResponseMessage(
            EndpointId origin, 
            MessageId id,
            MessageId inResponseTo, 
            InteractionConnectionState state) 
            : base(origin, id, inResponseTo)
        {
            m_State = state;
        }

        /// <summary>
        /// Gets a value that indicates if the sending endpoint is interested in maintaining the
        /// connection or not.
        /// </summary>
        public InteractionConnectionState State
        {
            [DebuggerStepThrough]
            get
            {
                return m_State;
            }
        }
    }
}
