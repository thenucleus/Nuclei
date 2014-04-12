//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="ConnectionVerificationMessage"/>.
    /// </summary>
    internal sealed class ConnectionVerificationProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly Action<EndpointId, ICommunicationMessage> m_SendMessage;

        /// <summary>
        /// The function that is used to generate the response data for a keep-alive message.
        /// </summary>
        private readonly KeepAliveResponseCustomDataBuilder m_ResponseDataBuilder;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <param name="responseDataBuilder">The function that is used to generate the response data for a keep-alive message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public ConnectionVerificationProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            SystemDiagnostics systemDiagnostics,
            KeepAliveResponseCustomDataBuilder responseDataBuilder = null)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendMessage);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
            m_Diagnostics = systemDiagnostics;
            m_ResponseDataBuilder = responseDataBuilder;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(ConnectionVerificationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "This method should not kill the application, it should notify the other side of the connection of failure if possible")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as ConnectionVerificationMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            // Process message and send response
            object responseData = null;
            if (m_ResponseDataBuilder != null)
            {
                responseData = m_ResponseDataBuilder(msg.CustomData);
            }

            try
            {
                var response = new ConnectionVerificationResponseMessage(m_Current, message.Id, responseData);
                m_SendMessage(message.Sender, response);
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to send keep-alive message response. Exception is: {0}",
                        e));
            }
        }
    }
}
