//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about the mapping of a single event of a <see cref="INotificationSet"/>.
    /// </summary>
    internal sealed class NotificationDefinition
    {
        /// <summary>
        /// The ID of the notification.
        /// </summary>
        private readonly NotificationId m_Id;

        /// <summary>
        /// The collection of event handlers that need to be notified when the notification is raised.
        /// </summary>
        private readonly List<Action<NotificationId, EventArgs>> m_EventHandlers
            = new List<Action<NotificationId, EventArgs>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDefinition"/> class.
        /// </summary>
        /// <param name="id">The notification ID for the notification on the <see cref="INotificationSet"/>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        public NotificationDefinition(NotificationId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
            }

            m_Id = id;
        }

        public void HandleEventAndForwardToListeners(object sender, EventArgs args)
        {
            foreach (var handler in m_EventHandlers)
            {
                handler(Id, args);
            }
        }

        /// <summary>
        /// Gets the ID of the notification.
        /// </summary>
        public NotificationId Id
        {
            [DebuggerStepThrough]
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Stores the handler for later use when a notification is raised.
        /// </summary>
        /// <param name="notificationHandler">The notification handler.</param>
        public void OnNotification(Action<NotificationId, EventArgs> notificationHandler)
        {
            {
                Lokad.Enforce.Argument(() => notificationHandler);
            }

            m_EventHandlers.Add(notificationHandler);
        }
    }
}
