//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Defines a message that is used to verify the status of a connection.
    /// </summary>
    internal sealed class ConnectionVerificationMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="customData">The custom data for the current message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        public ConnectionVerificationMessage(EndpointId origin, object customData = null) 
            : this(origin, new MessageId(), customData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="customData">The custom data for the current message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        public ConnectionVerificationMessage(EndpointId origin, MessageId id, object customData = null)
            : base(origin, id)
        {
            CustomData = customData;
        }

        /// <summary>
        /// Gets the custom data for the message.
        /// </summary>
        public object CustomData
        {
            get;
            private set;
        }
    }
}
