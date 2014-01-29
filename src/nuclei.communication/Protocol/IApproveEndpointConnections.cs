//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that determine if a remote endpoint is allowed to connect
    /// to the current endpoint.
    /// </summary>
    internal interface IApproveEndpointConnections
    {
        /// <summary>
        /// The version of the protocol for which the current instance can approve endpoint connections.
        /// </summary>
        Version ProtocolVersion
        {
            get;
        }

        /// <summary>
        /// Returns a value indicating whether the given remote endpoint is allowed to connect to the
        /// current endpoint.
        /// </summary>
        /// <param name="information">The connection description for the remote endpoint.</param>
        /// <returns>
        /// <see langword="true"/> if the remote endpoint is allowed to connect to the current endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointAllowedToConnect(CommunicationDescription information);
    }
}