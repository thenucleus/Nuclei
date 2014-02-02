//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines a <see cref="EventArgs"/> class that stores information about the availability of
    /// notifications for a communication endpoint.
    /// </summary>
    public sealed class NotificationSetAvailabilityEventArgs : EventArgs
    {
        /// <summary>
        /// The endpoint ID for the endpoint that has provided a set of commands.
        /// </summary>
        private readonly EndpointId m_Endpoint;

        /// <summary>
        /// The notifications that were provided by the endpoint.
        /// </summary>
        private readonly IEnumerable<Type> m_Notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSetAvailabilityEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that has provided a set of notifications.</param>
        /// <param name="notifications">The notifications that were provided.</param>
        public NotificationSetAvailabilityEventArgs(EndpointId endpoint, IEnumerable<Type> notifications)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => notifications);
            }

            m_Endpoint = endpoint;
            m_Notifications = notifications;
        }

        /// <summary>
        /// Gets the ID of the endpoint that provided the notifications.
        /// </summary>
        public EndpointId Endpoint
        {
            get
            {
                return m_Endpoint;
            }
        }

        /// <summary>
        /// Gets the collection containing the notification types that the endpoint provided.
        /// </summary>
        public IEnumerable<Type> Notifications
        {
            get
            {
                return m_Notifications;
            }
        }
    }
}
