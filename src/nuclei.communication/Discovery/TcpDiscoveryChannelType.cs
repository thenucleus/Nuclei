//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Configuration;

namespace Nuclei.Communication.Discovery
{
    internal sealed class TcpDiscoveryChannelType : TcpChannelType, IDiscoveryChannelType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannelType"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpDiscoveryChannelType(IConfiguration tcpConfiguration) 
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
        public ServiceEndpoint AttachDiscoveryEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint)
        {
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateBinding(), GenerateNewDiscoveryAddress());
            var discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
            host.Description.Behaviors.Add(discoveryBehavior);
            host.Description.Endpoints.Add(new UdpDiscoveryEndpoint());

            var endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
            endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("EndpointId", localEndpoint.ToString())));
            endpoint.Behaviors.Add(endpointDiscoveryBehavior);

            return endpoint;
        }

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        private string GenerateNewDiscoveryAddress()
        {
            return CommunicationConstants.DefaultDiscoveryAddressTemplate;
        }
    }
}
