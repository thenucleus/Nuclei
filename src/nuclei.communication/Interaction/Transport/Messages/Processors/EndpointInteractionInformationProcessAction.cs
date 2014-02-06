//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="CommandInvokedMessage"/>.
    /// </summary>
    internal sealed class EndpointInteractionInformationProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that stores the command proxy objects.
        /// </summary>
        private readonly IStoreRemoteCommandProxies m_CommandProxyHub;

        /// <summary>
        /// The object that stores the notification proxy objects.
        /// </summary>
        private readonly IStoreRemoteNotificationProxies m_NotificationProxyHub;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInteractionInformationProcessAction"/> class.
        /// </summary>
        /// <param name="commandProxyHub">The object that stores the command proxy objects.</param>
        /// <param name="notificationProxyHub">The object that stores the notification proxy objects.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandProxyHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationProxyHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public EndpointInteractionInformationProcessAction(
            IStoreRemoteCommandProxies commandProxyHub, 
            IStoreRemoteNotificationProxies notificationProxyHub,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => commandProxyHub);
                Lokad.Enforce.Argument(() => notificationProxyHub);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_CommandProxyHub = commandProxyHub;
            m_NotificationProxyHub = notificationProxyHub;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(EndpointInteractionInformationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Letting the exception escape will just kill the channel then we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as EndpointInteractionInformationMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            m_Diagnostics.Log(
                LevelToLog.Trace,
                CommunicationConstants.DefaultLogTextPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Received command and notification types from {0}",
                    msg.Sender));

            try
            {
                m_CommandProxyHub.OnReceiptOfEndpointCommands(msg.Sender, msg.Commands);
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to process the command types from {0}. Exception is: {1}",
                        message.Sender,
                        e));
            }

            try
            {
                m_NotificationProxyHub.OnReceiptOfEndpointNotifications(msg.Sender, msg.Notifications);
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to process the notification types from {0}. Exception is: {1}",
                        message.Sender,
                        e));
            }
        }
    }
}
