//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that is used to request registration to a notification.
    /// </summary>
    [Serializable]
    internal sealed class RegisterForNotificationMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterForNotificationMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="notificationToSubscribeTo">The notification to which the sender wants to subscribe.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationToSubscribeTo"/> is <see langword="null" />.
        /// </exception>
        public RegisterForNotificationMessage(EndpointId origin, NotificationData notificationToSubscribeTo)
            : base(origin)
        {
            {
                Lokad.Enforce.Argument(() => notificationToSubscribeTo);
            }

            Notification = notificationToSubscribeTo;
        }

        /// <summary>
        /// Gets the notification to which the sender of the current message wants to subscribe.
        /// </summary>
        public NotificationData Notification 
        { 
            get; 
            private set; 
        }
    }
}
