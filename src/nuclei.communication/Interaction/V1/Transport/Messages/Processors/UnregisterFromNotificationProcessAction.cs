//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction.Transport.V1.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="UnregisterFromNotificationMessage"/>.
    /// </summary>
    internal sealed class UnregisterFromNotificationProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that stores the notification registrations.
        /// </summary>
        private readonly ISendNotifications m_NotificationSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisterFromNotificationProcessAction"/> class.
        /// </summary>
        /// <param name="notificationSender">The object that stores the registrations.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationSender"/> is <see langword="null" />.
        /// </exception>
        public UnregisterFromNotificationProcessAction(ISendNotifications notificationSender)
        {
            {
                Lokad.Enforce.Argument(() => notificationSender);
            }

            m_NotificationSender = notificationSender;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(UnregisterFromNotificationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as UnregisterFromNotificationMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            m_NotificationSender.UnregisterFromNotification(msg.Sender, msg.Notification);
        }
    }
}
