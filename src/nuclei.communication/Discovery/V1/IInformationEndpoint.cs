//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net.Security;
using System.ServiceModel;
using ProtoBuf;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines the interface for objects that provide the methods that can be called on the discovery channel.
    /// </summary>
    [ServiceContract]
    internal interface IInformationEndpoint : IVersionedDiscoveryEndpoint
    {
        /// <summary>
        /// Returns an array containing all the versions of the supported communication protocols.
        /// </summary>
        /// <returns>An array containing the versions of the supported communication protocols.</returns>
        [OperationContract(
            IsOneWay = false,
            IsInitiating = true,
            IsTerminating = false,
            ProtectionLevel = ProtectionLevel.None)]
        Version[] ProtocolVersions();

        /// <summary>
        /// Returns the discovery information for the communication protocol with the given version.
        /// </summary>
        /// <param name="version">The version of the protocol for which the discovery information should be provided.</param>
        /// <returns>The discovery information for the communication protocol with the given version.</returns>
        [OperationContract(
            IsOneWay = false,
            IsInitiating = true,
            IsTerminating = false,
            ProtectionLevel = ProtectionLevel.None)]
        VersionedChannelInformation ConnectionInformationForProtocol(Version version);
    }
}
