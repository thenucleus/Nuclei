//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net.Security;
using System.ServiceModel;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the base interface for version specific discovery WCF endpoints.
    /// </summary>
    internal interface IVersionedDiscoveryEndpoint : IDiscoveryEndpoint
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
        Version Version();
    }
}
