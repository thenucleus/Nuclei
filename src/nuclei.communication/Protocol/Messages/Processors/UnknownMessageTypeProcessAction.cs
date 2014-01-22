//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an any kind of <see cref="ICommunicationMessage"/>
    /// for which no filter has been registered.
    /// </summary>
    [LastChanceMessageHandler]
    internal sealed class UnknownMessageTypeProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly Action<EndpointId, ICommunicationMessage> m_SendMessage;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMessageTypeProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public UnknownMessageTypeProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendMessage);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(ICommunicationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "If the exception escapes then the channel dies but we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as UnknownMessageTypeMessage;
            if (msg != null)
            {
                // We don't want to respond to our own messages otherwise we get a nasty
                // feedback loop that just keeps going, and going and going and going and
                // .... well I think you get the point.
                return;
            }

            try
            {
                var returnMessage = new UnknownMessageTypeMessage(m_Current, message.Id);
                m_SendMessage(message.Sender, returnMessage);
            }
            catch (Exception e)
            {
                try
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while sending a message indicating that received message was of an unknown type. Exception is: {0}",
                            e));
                    m_SendMessage(message.Sender, new FailureMessage(m_Current, message.Id));
                }
                catch (Exception errorSendingException)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while trying to send unknown message response. Exception is: {0}",
                            errorSendingException));
                }
            }
        }
    }
}
