//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that store information about all known endpoints.
    /// </summary>
    internal interface IStoreInformationAboutEndpoints : INotifyOfEndpointStateChange, IEnumerable<EndpointId>
    {
        /// <summary>
        /// Indicates if it is possible and allowed to communicate with a given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if it is possible and allowed to communicate with the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanCommunicateWithEndpoint(EndpointId endpoint);

        /// <summary>
        /// Returns the connection information for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <param name="information">The connection information for the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the information for the endpoint was retrieved successfully; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryGetConnectionFor(EndpointId endpoint, out EndpointInformation information);

        /// <summary>
        /// Removes the endpoint from the storage.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the endpoint was removed successfully; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryRemoveEndpoint(EndpointId endpoint);
    }
}
