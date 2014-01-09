//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Configuration;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses TCP/IP connections for communication between
    /// applications different machines.
    /// </summary>
    internal sealed class TcpChannelType : IChannelType
    {
        /// <summary>
        /// Returns the DNS name of the machine.
        /// </summary>
        /// <returns>The DNS name of the machine.</returns>
        private static string MachineDnsName()
        {
            try
            {
                var searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE ServiceName != 'tunnel' AND DNSHostName != null");

                string dnsHostName = (from ManagementObject queryObj in searcher.Get()
                                      select queryObj["DNSHostName"] as string).FirstOrDefault();

                return (!string.IsNullOrWhiteSpace(dnsHostName)) ? dnsHostName : Environment.MachineName;
            }
            catch (ManagementException)
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Returns the next available TCP/IP port.
        /// </summary>
        /// <returns>
        /// The number of the port.
        /// </returns>
        private static int DetermineNextAvailablePort()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(endPoint);
                var local = (IPEndPoint)socket.LocalEndPoint;
                return local.Port;
            }
        }

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
        /// Initializes a new instance of the <see cref="TcpChannelType"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <param name="shouldProvideDiscovery">
        ///     A flag that indicates if the TCP channels should participate in the UDP discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpChannelType(IConfiguration tcpConfiguration, bool shouldProvideDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => tcpConfiguration);
            }

            m_Configuration = tcpConfiguration;
            m_ShouldProvideDiscovery = shouldProvideDiscovery;
        }

        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.TcpIP;
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
            int port = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpPort) ?
                m_Configuration.Value<int>(CommunicationConfigurationKeys.TcpPort) : 
                DetermineNextAvailablePort();
            string address = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBaseAddress) ? 
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpBaseAddress) : 
                MachineDnsName();

            var channelUri = string.Format(CultureInfo.InvariantCulture, CommunicationConstants.DefaultTcpIpChannelUriTemplate, address, port);
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
            var binding = new NetTcpBinding(SecurityMode.None, false)
                {
                    MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        : CommunicationConstants.DefaultMaximumNumberOfConnectionsForTcpIp,
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
            var binding = new NetTcpBinding(SecurityMode.None, false)
                {
                    MaxConnections = m_Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        ? m_Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        : CommunicationConstants.DefaultMaximumNumberOfConnectionsForTcpIp,
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

        /// <summary>
        /// Generates a new address for the channel endpoint.
        /// </summary>
        /// <returns>
        /// The newly generated address for the channel endpoint.
        /// </returns>
        private string GenerateNewMessageAddress()
        {
            return m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpSubaddress) ?
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpSubaddress) :
                string.Format(CultureInfo.InvariantCulture, CommunicationConstants.DefaultTcpIpAddressTemplate, CurrentProcessId());
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
