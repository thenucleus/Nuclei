//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Configuration;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses named pipes for communication between
    /// applications on the same local machine.
    /// </summary>
    internal sealed class NamedPipeChannelType : IChannelType
    {
        /// <summary>
        /// Returns the process ID of the process that is currently executing
        /// this code.
        /// </summary>
        /// <returns>
        /// The ID number of the current process.
        /// </returns>
        private static int CurrentProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        /// <summary>
        /// The object that stores the configuration values for the
        /// named pipe WCF connection.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// A flag that indicates if the channel should participate in the UDP 
        /// discovery or not.
        /// </summary>
        private readonly bool m_ShouldProvideDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeChannelType"/> class.
        /// </summary>
        /// <param name="namedPipeConfiguration">The configuration for the WCF named pipe channel.</param>
        /// <param name="shouldProvideDiscovery">
        ///     A flag that indicates if the named pipe channels should participate in the UDP discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="namedPipeConfiguration"/> is <see langword="null" />.
        /// </exception>
        public NamedPipeChannelType(IConfiguration namedPipeConfiguration, bool shouldProvideDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => namedPipeConfiguration);
            }

            m_Configuration = namedPipeConfiguration;
            m_ShouldProvideDiscovery = shouldProvideDiscovery;
        }

        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.NamedPipe;
            }
        }

        /// <summary>
        /// Generates a new URI for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated URI.
        /// </returns>
        public Uri GenerateNewChannelUri()
        {
            var channelUri = string.Format(
                CultureInfo.InvariantCulture, 
                CommunicationConstants.DefaultNamedPipeChannelUriTemplate, 
                CurrentProcessId());
            return new Uri(channelUri);
        }

        /// <summary>
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateMessageBinding()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None) 
                { 
                    MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        : CommunicationConstants.DefaultMaximumNumberOfConnectionsForNamedPipes,
                    ReceiveTimeout = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds) 
                        ? TimeSpan.FromMilliseconds(m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds)) 
                        : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultBindingReceiveTimeoutInMilliSeconds),
                    MaxBufferSize = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxBufferSizeForMessagesInBytes)
                        ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaxBufferSizeForMessagesInBytes)
                        : CommunicationConstants.DefaultBindingMaxBufferSizeForMessagesInBytes,
                    MaxReceivedMessageSize = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxReceivedSizeForMessagesInBytes)
                        ? m_Configuration.Value<long>(CommunicationConfigurationKeys.BindingMaxReceivedSizeForMessagesInBytes)
                        : CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                    TransferMode = TransferMode.Buffered,
                };

            return binding;
        }

        /// <summary>
        /// Generates a new binding object used to transfer data across the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateDataBinding()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                    ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                    : CommunicationConstants.DefaultMaximumNumberOfConnectionsForNamedPipes,
                ReceiveTimeout = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds) 
                    ? TimeSpan.FromMilliseconds(m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds)) 
                    : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultBindingReceiveTimeoutInMilliSeconds),
                MaxBufferSize = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxBufferSizeForDataInBytes)
                    ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaxBufferSizeForDataInBytes)
                    : CommunicationConstants.DefaultBindingMaxBufferSizeForMessagesInBytes,
                MaxReceivedMessageSize = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxReceivedSizeForDataInBytes)
                    ? m_Configuration.Value<long>(CommunicationConfigurationKeys.BindingMaxReceivedSizeForDataInBytes)
                    : CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                TransferMode = TransferMode.Streamed,
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
        public ServiceEndpoint AttachMessageEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint)
        {
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateMessageBinding(), GenerateNewMessageAddress());
            if (m_ShouldProvideDiscovery)
            {
                var discoveryBehavior = new ServiceDiscoveryBehavior();
                discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
                host.Description.Behaviors.Add(discoveryBehavior);
                host.Description.Endpoints.Add(new UdpDiscoveryEndpoint());

                var endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
                endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("EndpointId", localEndpoint.ToString())));
                endpointDiscoveryBehavior.Extensions.Add(new XElement("root", new XElement("BindingType", ChannelType)));
                endpoint.Behaviors.Add(endpointDiscoveryBehavior);
            }

            return endpoint;
        }

        private string GenerateNewMessageAddress()
        {
            return m_Configuration.HasValueFor(CommunicationConfigurationKeys.NamedPipeSubaddress) ?
                m_Configuration.Value<string>(CommunicationConfigurationKeys.NamedPipeSubaddress) :
                string.Format(CultureInfo.InvariantCulture, CommunicationConstants.DefaultNamedPipeAddressTemplate, CurrentProcessId());
        }

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <returns>The newly attached endpoint.</returns>
        public ServiceEndpoint AttachDataEndpoint(ServiceHost host, Type implementedContract)
        {
            return host.AddServiceEndpoint(implementedContract, GenerateDataBinding(), GenerateNewDataAddress());
        }

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        private string GenerateNewDataAddress()
        {
            var subAddress = GenerateNewMessageAddress();
            return string.Format(CultureInfo.InvariantCulture, CommunicationConstants.DefaultDataAddressPostfixTemplate, subAddress);
        }
    }
}
