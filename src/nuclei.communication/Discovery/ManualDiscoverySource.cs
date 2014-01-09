//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Handles the discovery of endpoints by accepting endpoint information from
    /// external sources.
    /// </summary>
    internal sealed class ManualDiscoverySource : IDiscoverOtherServices, IAcceptExternalEndpointInformation
    {
        /// <summary>
        /// Indicates if discovery information should be passed on or not.
        /// </summary>
        private bool m_IsDiscoveryAllowed;

        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<EndpointDiscoveredEventArgs> OnEndpointBecomingAvailable;

        private void RaiseOnEndpointBecomingAvailable(ChannelConnectionInformation info)
        {
            var local = OnEndpointBecomingAvailable;
            if (local != null)
            {
                local(this, new EndpointDiscoveredEventArgs(info));
            }
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        public event EventHandler<EndpointLostEventArgs> OnEndpointBecomingUnavailable;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "We need this method because otherwise the compiler throws an error.")]
        private void RaiseOnEndpointBecomingUnavailable(EndpointId id, ChannelType channelType)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointLostEventArgs(id, channelType));
            }
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public void StartDiscovery()
        {
            m_IsDiscoveryAllowed = true;
        }

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        public void EndDiscovery()
        {
            m_IsDiscoveryAllowed = false;
        }

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// connected to the network.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="channelType">The kind of channel this connection information describes.</param>
        /// <param name="address">The full URI for the channel.</param>
        public void RecentlyConnectedEndpoint(EndpointId id, ChannelType channelType, Uri address)
        {
            if (!m_IsDiscoveryAllowed)
            {
                return;
            }

            RaiseOnEndpointBecomingAvailable(new ChannelConnectionInformation(id, channelType, address));
        }

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// disconnected from the network.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="channelType">The kind of channel this connection information describes.</param>
        public void RecentlyDisconnectedEndpoint(EndpointId id, ChannelType channelType)
        {
            if (!m_IsDiscoveryAllowed)
            {
                return;
            }

            RaiseOnEndpointBecomingUnavailable(id, channelType);
        }
    }
}
