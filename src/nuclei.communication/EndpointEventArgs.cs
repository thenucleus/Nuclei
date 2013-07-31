//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that provides the ID number of an endpoint.
    /// </summary>
    public sealed class EndpointEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public EndpointEventArgs(EndpointId endpoint)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
            }

            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets a value indicating the ID of the affected endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }
    }
}
