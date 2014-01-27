//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Defines the base methods for <see cref="ICommunicationMessage"/> objects.
    /// </summary>
    internal abstract class CommunicationMessage : ICommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        protected CommunicationMessage(EndpointId origin)
            : this(origin, MessageId.None)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        protected CommunicationMessage(EndpointId origin, MessageId inResponseTo)
        {
            {
                Lokad.Enforce.Argument(() => origin);
                Lokad.Enforce.Argument(() => inResponseTo);
            }

            Id = new MessageId();
            InResponseTo = inResponseTo;
            Sender = origin;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the message.
        /// </summary>
        public MessageId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the message to which 
        /// the current message is a response.
        /// </summary>
        public MessageId InResponseTo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the endpoint that 
        /// send the current message.
        /// </summary>
        public EndpointId Sender
        {
            get;
            private set;
        }
    }
}
