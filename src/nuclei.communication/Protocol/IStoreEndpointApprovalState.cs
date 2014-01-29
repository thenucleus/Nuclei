//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that store endpoint approval information for all known endpoints.
    /// </summary>
    internal interface IStoreEndpointApprovalState : IStoreInformationAboutEndpoints
    {
        /// <summary>
        /// Add a newly discovered endpoint to the collection.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <param name="connection">The connection information for the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the endpoint was added; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryAdd(EndpointId endpoint, EndpointInformation connection);

        /// <summary>
        /// Starts the approval process.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <param name="description">The communication information for the remote endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the approval process was successfully started; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryStartApproval(EndpointId endpoint, CommunicationDescription description);

        /// <summary>
        /// Completes the approval of the endpoint.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the endpoint was approved; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryCompleteApproval(EndpointId endpoint);

        /// <summary>
        /// Updates the connection information stored for the given endpoint.
        /// </summary>
        /// <param name="connectionInformation">The new connection information.</param>
        /// <returns>
        /// <see langword="true" /> if the endpoint information was updated; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool TryUpdate(EndpointInformation connectionInformation);

        /// <summary>
        /// Indicates if the endpoint has been contacted, but the approval process hasn't been started yet.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the given endpoint has been contacted but has not been approved for communication; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasBeenContacted(EndpointId endpoint);

        /// <summary>
        /// Indicates if the endpoint has been discovered but not approved yet.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if the given endpoint has been discovered but has not been approved for communication; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsWaitingForApproval(EndpointId endpoint);

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
