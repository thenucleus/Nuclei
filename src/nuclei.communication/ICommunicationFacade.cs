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
    /// Defines the interface for objects that provide the user-end for the communication system.
    /// </summary>
    public interface ICommunicationFacade : INotifyOfEndpointStateChange
    {
        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Returns a collection containing all the known remote endpoints.
        /// </summary>
        /// <returns>The collection of all the known remote endpoints.</returns>
        IEnumerable<EndpointId> KnownEndpoints();

        /// <summary>
        /// Gets the endpoint ID for the endpoint with its discovery channel at the given URI.
        /// </summary>
        /// <param name="address">The URI of the discovery channel.</param>
        /// <returns>The endpoint ID of the endpoint with a discovery channel at the given URI.</returns>
        EndpointId FromUri(Uri address);
    }
}
