//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Handles the discovery of endpoints by accepting endpoint information from
    /// external sources.
    /// </summary>
    internal sealed class ManualDiscoverySource : DiscoverySource, IAcceptExternalEndpointInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualDiscoverySource"/> class.
        /// </summary>
        /// <param name="translatorsByVersion">
        ///     An array containing all the supported translators mapped to the version of the discovery layer.
        /// </param>
        /// <param name="template">The channel type that is used to create WCF channels.</param>
        /// <param name="diagnostics">The object that provides the discovery information for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="translatorsByVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="template"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public ManualDiscoverySource(
            Tuple<Version, ITranslateVersionedChannelInformation>[] translatorsByVersion, 
            IDiscoveryChannelTemplate template, 
            SystemDiagnostics diagnostics) : base(translatorsByVersion, template, diagnostics)
        {
        }

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
