//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that handle handshake protocols.
    /// </summary>
    internal interface IHandleHandshakes
    {
        /// <summary>
        /// Gets the version of the protocol that is supported by the current
        /// handshake handler.
        /// </summary>
        Version SupportedProtocolVersion
        {
            get;
        }

        /// <summary>
        /// Continues the handshake process between the current endpoint and the specified endpoint.
        /// </summary>
        /// <param name="connection">The connection information for endpoint that started the handshake.</param>
        /// <param name="information">The handshake information for the endpoint.</param>
        /// <param name="messageId">The ID of the message that carried the handshake information.</param>
        void ContinueHandshakeWith(
            EndpointInformation connection, 
            CommunicationDescription information, 
            MessageId messageId);
    }
}
