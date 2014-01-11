//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that stores information about an endpoint that disappeared from
    /// the network.
    /// </summary>
    internal sealed class EndpointLostEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointLostEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that disappeared.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public EndpointLostEventArgs(EndpointId endpoint)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
            }

            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the ID of the endpoint that disappeared.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }
    }
}
