//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net.Security;
using System.ServiceModel;
using ProtoBuf;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for endpoints that provide the bootstrapping capability
    /// for the discovery process.
    /// </summary>
    [ServiceContract]
    [ProtoContract]
    interface IBootstrapEndpoint : IDiscoveryEndpoint
    {
        /// <summary>
        /// Returns the supported versions of the discovery layer.
        /// </summary>
        /// <returns>A collection containing all the supported versions of the discovery layer.</returns>
        [OperationContract(
            IsOneWay = false,
            IsInitiating = true,
            IsTerminating = false,
            ProtectionLevel = ProtectionLevel.None)]
        Version[] DiscoveryVersions();

        /// <summary>
        /// Returns the URI of the discovery channel with the given version.
        /// </summary>
        /// <param name="version">The version of the discovery channel.</param>
        /// <returns>The URI of the discovery channel with the given version.</returns>
        Uri UriForVersion(Version version);
    }
}
