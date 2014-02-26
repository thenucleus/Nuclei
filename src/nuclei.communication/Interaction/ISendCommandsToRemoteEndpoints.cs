//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that handle sending commands to remote endpoints.
    /// </summary>
    public interface ISendCommandsToRemoteEndpoints : INotifyOfEndpointStateChange
    {
        /// <summary>
        /// Returns a value indicating if a specific set of commands is available for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="commandInterfaceType">The type of the command that should be available.</param>
        /// <returns>
        ///     <see langword="true" /> if there are the specific commands exist for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasCommandFor(EndpointId endpoint, Type commandInterfaceType);

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TCommand">The typeof command set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <returns>The requested command set.</returns>
        TCommand CommandsFor<TCommand>(EndpointId endpoint) where TCommand : class, ICommandSet;

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <returns>The requested command set.</returns>
        ICommandSet CommandsFor(EndpointId endpoint, Type commandType);
    }
}
