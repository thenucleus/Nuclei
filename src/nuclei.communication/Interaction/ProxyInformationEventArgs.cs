//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores <see cref="EventArgs"/> describing the registration of a new proxy on a
    /// remote endpoint.
    /// </summary>
    internal sealed class ProxyInformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInformationEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint on which the proxy was registered.</param>
        /// <param name="proxyType">The command that was registered.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyType"/> is <see langword="null" />.
        /// </exception>
        public ProxyInformationEventArgs(EndpointId endpoint, ISerializedType proxyType)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => proxyType);
            }

            Endpoint = endpoint;
            Proxy = proxyType;
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
        /// Gets the proxy that was registered on the remote endpoint.
        /// </summary>
        public ISerializedType Proxy
        {
            get;
            private set;
        }
    }
}
