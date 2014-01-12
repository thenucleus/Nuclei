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
    /// Defines a <see cref="IProtocolChannelType"/> that uses TCP/IP connections for communication between
    /// applications on different machines.
    /// </summary>
    internal sealed class TcpProtocolChannelType : TcpChannelType, IProtocolChannelType
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
        /// Initializes a new instance of the <see cref="TcpProtocolChannelType"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        public TcpProtocolChannelType(IConfiguration tcpConfiguration)
            : base(tcpConfiguration)
        {
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
        /// Generates a new binding object for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateMessageBinding()
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
        /// Generates a new binding object used to transfer data across the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        public Binding GenerateDataBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None, false)
                {
                    MaxConnections = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections)
                        ? Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaximumNumberOfConnections) 
                        : CommunicationConstants.DefaultMaximumNumberOfConnectionsForTcpIp,
                    ReceiveTimeout = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds)
                        ? TimeSpan.FromMilliseconds(Configuration.Value<int>(CommunicationConfigurationKeys.BindingReceiveTimeoutInMilliseconds)) 
                        : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultBindingReceiveTimeoutInMilliSeconds),
                    MaxBufferSize = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxBufferSizeForDataInBytes)
                        ? Configuration.Value<int>(CommunicationConfigurationKeys.BindingMaxBufferSizeForDataInBytes)
                        : CommunicationConstants.DefaultBindingMaxBufferSizeForMessagesInBytes,
                    MaxReceivedMessageSize = Configuration.HasValueFor(CommunicationConfigurationKeys.BindingMaxReceivedSizeForDataInBytes)
                        ? Configuration.Value<long>(CommunicationConfigurationKeys.BindingMaxReceivedSizeForDataInBytes)
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
            return Configuration.HasValueFor(CommunicationConfigurationKeys.TcpSubaddress) ?
                Configuration.Value<string>(CommunicationConfigurationKeys.TcpSubaddress) :
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
