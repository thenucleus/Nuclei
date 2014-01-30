//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the methods for objects that handle discovery of endpoints.
    /// </summary>
    internal interface IDiscoverOtherServices
    {
        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        event EventHandler<EndpointDiscoveredEventArgs> OnEndpointBecomingAvailable;

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointBecomingUnavailable;

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        void StartDiscovery();

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        void EndDiscovery();
    }
}
