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
    internal sealed class ConnectionVerificationResponseMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <param name="customData">The custom data for the current message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public ConnectionVerificationResponseMessage(EndpointId origin, MessageId inResponseTo, object customData = null) 
            : this(origin, new MessageId(), inResponseTo, customData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <param name="customData">The custom data for the current message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public ConnectionVerificationResponseMessage(EndpointId origin, MessageId id, MessageId inResponseTo, object customData = null)
            : base(origin, id, inResponseTo)
        {
            ResponseData = customData;
        }

        /// <summary>
        /// Gets the custom data for the message.
        /// </summary>
        public object ResponseData
        {
            get;
            private set;
        }
    }
}
