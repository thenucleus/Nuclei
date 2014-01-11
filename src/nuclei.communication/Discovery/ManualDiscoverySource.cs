//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Handles the discovery of endpoints by accepting endpoint information from
    /// external sources.
    /// </summary>
    internal sealed class ManualDiscoverySource : DiscoverySource, IAcceptExternalEndpointInformation
    {
        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// connected to the network.
        /// </summary>
        /// <param name="id">The ID of the recently discovered endpoint.</param>
        /// <param name="address">The full URI for the discovery channel of the endpoint.</param>
        public void RecentlyConnectedEndpoint(EndpointId id, Uri address)
        {
            if (!IsDiscoveryAllowed)
            {
                return;
            }

            LocatedRemoteEndpointOnChannel(id, address);
        }

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// disconnected from the network.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        public void RecentlyDisconnectedEndpoint(EndpointId id)
        {
            if (!IsDiscoveryAllowed)
            {
                return;
            }

            LostRemoteEndpointWithId(id);
        }
    }
}
