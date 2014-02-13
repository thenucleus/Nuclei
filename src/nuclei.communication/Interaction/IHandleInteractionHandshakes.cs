//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that handle the handshake process for the interaction layer.
    /// </summary>
    internal interface IHandleInteractionHandshakes
    {
        /// <summary>
        /// Continues the handshake process between the current endpoint and the specified endpoint.
        /// </summary>
        /// <param name="connection">The ID of the endpoint that started the handshake.</param>
        /// <param name="subjectGroups">The handshake information for the endpoint.</param>
        /// <param name="messageId">The ID of the message that carried the handshake information.</param>
        void ContinueHandshakeWith(
            EndpointId connection,
            CommunicationSubjectGroup[] subjectGroups,
            MessageId messageId);
    }
}
