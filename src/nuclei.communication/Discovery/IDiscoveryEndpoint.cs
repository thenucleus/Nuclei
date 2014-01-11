//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net.Security;
using System.ServiceModel;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for objects that provide the methods that can be called on the discovery channel.
    /// </summary>
    [ServiceContract]
    internal interface IDiscoveryEndpoint : IReceiveInformationFromRemoteEndpoints
    {
        /// <summary>
        /// Returns the version of the discovery protocol.
        /// </summary>
        /// <returns>The version of the discovery protocol.</returns>
        [OperationContract(
            IsOneWay = false,
            IsInitiating = true,
            IsTerminating = false,
            ProtectionLevel = ProtectionLevel.None)]
        Version DiscoveryVersion();

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
        IDiscoveryInformation ConnectionInformationForProtocol(Version version);
    }
}
