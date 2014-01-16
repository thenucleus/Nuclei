//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the channel type for discovery purposes.
    /// </summary>
    internal interface IDiscoveryChannelTemplate : IChannelTemplate
    {
        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        Binding GenerateBinding();

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="localEndpoint">The ID of the local endpoint, to be used in the endpoint metadata.</param>
        /// <returns>The newly attached endpoint.</returns>
        ServiceEndpoint AttachDiscoveryEntryEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint);

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="version">The version of the discovery endpoint.</param>
        /// <returns>The newly attached endpoint.</returns>
        ServiceEndpoint AttachVersionedDiscoveryEndpoint(ServiceHost host, Type implementedContract, Version version);
    }
}
