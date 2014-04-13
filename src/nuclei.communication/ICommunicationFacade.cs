//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="id">The endpoint ID of the endpoint from which the current endpoint should disconnect.</param>
        void DisconnectFrom(EndpointId id);

        /// <summary>
        /// Gets the endpoint ID for the endpoint with its discovery channel at the given URI.
        /// </summary>
        /// <param name="address">The URI of the discovery channel.</param>
        /// <returns>The endpoint ID of the endpoint with a discovery channel at the given URI.</returns>
        EndpointId FromUri(Uri address);

        /// <summary>
        /// Verifies that the connection to the given endpoint can be used.
        /// </summary>
        /// <param name="id">The endpoint ID of the endpoint to which the connection should be verified.</param>
        /// <returns>
        /// <see langword="true" /> if the connection is active; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsConnectionActive(EndpointId id);

        /// <summary>
        /// Verifies that the connection to the endpoint at the given URL can be used.
        /// </summary>
        /// <param name="address">The discovery URL of the endpoint to which the connection should be verified.</param>
        /// <returns>
        /// <see langword="true" /> if the connection is active; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsConnectionActive(Uri address);
    }
}
