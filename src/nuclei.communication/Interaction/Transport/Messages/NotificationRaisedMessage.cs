//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that indicates that the an <see cref="INotificationSet"/> event was
    /// raised.
    /// </summary>
    internal sealed class NotificationRaisedMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="eventNotification">The information about the <see cref="INotificationSet"/> notification that was raised.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="eventNotification"/> is <see langword="null" />.
        /// </exception>
        public NotificationRaisedMessage(EndpointId origin, NotificationRaisedData eventNotification)
            : this(origin, new MessageId(), eventNotification)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="eventNotification">The information about the <see cref="INotificationSet"/> notification that was raised.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="eventNotification"/> is <see langword="null" />.
        /// </exception>
        public NotificationRaisedMessage(EndpointId origin, MessageId id, NotificationRaisedData eventNotification)
            : base(origin, id)
        {
            {
                Lokad.Enforce.Argument(() => eventNotification);
            }

            Notification = eventNotification;
        }

        /// <summary>
        /// Gets information about the <see cref="INotificationSet"/> notification that was raised.
        /// </summary>
        public NotificationRaisedData Notification
        {
            get;
            private set;
        }
    }
}
