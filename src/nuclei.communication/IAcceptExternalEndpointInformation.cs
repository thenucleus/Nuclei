//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that store or forward information about
    /// the connection state of external endpoints.
    /// </summary>
    internal interface IAcceptExternalEndpointInformation
    {
        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// connected to the network.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="channelType">The kind of channel this connection information describes.</param>
        /// <param name="address">The full URI for the channel.</param>
        void RecentlyConnectedEndpoint(EndpointId id, ChannelType channelType, Uri address);
    }
}
