//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Stores <see cref="EventArgs"/> describing the registration of a new notification on a
    /// remote endpoint.
    /// </summary>
    internal sealed class NotificationInformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInformationEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint on which the notification was registered.</param>
        /// <param name="notification">The notification that was registered.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notification"/> is <see langword="null" />.
        /// </exception>
        public NotificationInformationEventArgs(EndpointId endpoint, ISerializedType notification)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => notification);
            }

            Endpoint = endpoint;
            Notification = notification;
        }

        /// <summary>
        /// Gets the ID number of the remote endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the notification that was registered on the remote endpoint.
        /// </summary>
        public ISerializedType Notification
        {
            get;
            private set;
        }
    }
}
