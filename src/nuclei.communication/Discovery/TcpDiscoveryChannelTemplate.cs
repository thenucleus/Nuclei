//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Configuration;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines a TCP channel type for discovery channels.
    /// </summary>
    internal sealed class TcpDiscoveryChannelTemplate : TcpChannelTemplate, IDiscoveryChannelTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpDiscoveryChannelTemplate"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpDiscoveryChannelTemplate(IConfiguration tcpConfiguration) 
            : base(tcpConfiguration)
        {
        }

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None, false)
            {
                MaxConnections = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections)
                    ? Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections)
                    : CommunicationConstants.DefaultMaximumNumberOfConnectionsForTcpIp,
                ReceiveTimeout = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds)
                    ? TimeSpan.FromMilliseconds(Configuration.Value<int>(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds))
                    : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultBindingReceiveTimeoutInMilliSeconds),
                MaxBufferSize = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxBufferSizeForMessagesInBytes)
                    ? Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaxBufferSizeForMessagesInBytes)
                    : CommunicationConstants.DefaultBindingMaxBufferSizeForMessagesInBytes,
                MaxReceivedMessageSize = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxReceivedSizeForMessagesInBytes)
                    ? Configuration.Value<long>(CommunicationConfigurationKeys.BindingMaxReceivedSizeForMessagesInBytes)
                    : CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                TransferMode = TransferMode.Buffered,
            };

            return binding;
        }

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="localEndpoint">The ID of the local endpoint, to be used in the endpoint metadata.</param>
        /// <returns>The newly attached endpoint.</returns>
        public ServiceEndpoint AttachDiscoveryEntryEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint)
        {
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateBinding(), GenerateNewDiscoveryEntryAddress());
            var discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
            host.Description.Behaviors.Add(discoveryBehavior);
            host.Description.Endpoints.Add(new UdpDiscoveryEndpoint());

            var endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
            endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("EndpointId", localEndpoint.ToString())));
            endpoint.Behaviors.Add(endpointDiscoveryBehavior);
            endpoint.Behaviors.Add(new ProtoEndpointBehavior());

            return endpoint;
        }

        /// <summary>
        /// Generates a new address for the entry channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the entry channel endpoint.
        /// </returns>
        private string GenerateNewDiscoveryEntryAddress()
        {
            return Configuration.HasValueFor(CommunicationConfigurationKeys.TcpDiscoveryPath) ?
                Configuration.Value<string>(CommunicationConfigurationKeys.TcpDiscoveryPath) :
                CommunicationConstants.DefaultDiscoveryEntryAddressTemplate;
        }

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="version">The version of the discovery endpoint.</param>
        /// <returns>The newly attached endpoint.</returns>
        public ServiceEndpoint AttachVersionedDiscoveryEndpoint(ServiceHost host, Type implementedContract, Version version)
        {
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateBinding(), GenerateNewDiscoveryVersionedAddress(version));
            endpoint.Behaviors.Add(new ProtoEndpointBehavior());
            return endpoint;
        }

        /// <summary>
        /// Generates a new address for the versioned channel endpoint.
        /// </summary>
        /// <param name="version">The version for which the address needs to be determined.</param>
        /// <returns>
        /// The newly generated address for the versioned channel endpoint.
        /// </returns>
        private string GenerateNewDiscoveryVersionedAddress(Version version)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                @"{0}/V{1}",
                GenerateNewDiscoveryEntryAddress(),
                version.ToString(4));
        }
    }
}
