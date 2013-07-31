//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication
{
    /// <summary>
    /// Maps an endpoint to a set of registered notifications.
    /// </summary>
    public sealed class NotificationInformationPerEndpoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInformationPerEndpoint"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <param name="notifications">The collection that describes all the registered notifications.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        public NotificationInformationPerEndpoint(EndpointId endpoint, IEnumerable<Type> notifications)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => notifications);
            }

            Endpoint = endpoint;
            RegisteredNotifications = notifications;
        }

        /// <summary>
        /// Gets the ID number of the endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection that describes all the 
        /// registered notifications for the given endpoint.
        /// </summary>
        public IEnumerable<Type> RegisteredNotifications
        {
            get;
            private set;
        }
    }
}
