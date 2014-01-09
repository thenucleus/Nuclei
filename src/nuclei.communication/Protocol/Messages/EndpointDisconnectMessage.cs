//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint is about to disconnect
    /// from the network.
    /// </summary>
    [Serializable]
    internal sealed class EndpointDisconnectMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDisconnectMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        public EndpointDisconnectMessage(EndpointId origin)
            : this(origin, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDisconnectMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="closingReason">
        /// The reason the channel is about to be closed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closingReason"/> is <see langword="null" />.
        /// </exception>
        public EndpointDisconnectMessage(EndpointId origin, string closingReason)
            : base(origin)
        {
            {
                Lokad.Enforce.Argument(() => closingReason);
            }

            ClosingReason = closingReason;
        }

        /// <summary>
        /// Gets a value indicating why the channel is being closed.
        /// </summary>
        public string ClosingReason
        {
            get;
            private set;
        }
    }
}
