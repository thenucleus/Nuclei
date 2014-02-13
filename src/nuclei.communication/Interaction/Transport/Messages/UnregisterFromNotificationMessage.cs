﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that is used to request un-registration from a notification.
    /// </summary>
    internal sealed class UnregisterFromNotificationMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisterFromNotificationMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="notificationToUnsubscribeFrom">The notification from which the sender wants to unsubscribe.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationToUnsubscribeFrom"/> is <see langword="null" />.
        /// </exception>
        public UnregisterFromNotificationMessage(EndpointId origin, NotificationData notificationToUnsubscribeFrom)
            : base(origin)
        {
            {
                Lokad.Enforce.Argument(() => notificationToUnsubscribeFrom);
            }

            Notification = notificationToUnsubscribeFrom;
        }

        /// <summary>
        /// Gets the notification from which the sender of the current message wants to unsubscribe.
        /// </summary>
        public NotificationData Notification 
        { 
            get; 
            private set; 
        }
    }
}
