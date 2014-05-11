//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Nuclei.Configuration;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines a <see cref="IProtocolChannelTemplate"/> that uses TCP/IP connections for communication between
    /// applications on different machines.
    /// </summary>
    internal sealed class TcpProtocolChannelTemplate : TcpChannelTemplate, IProtocolChannelTemplate
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
        /// The data contract resolver that is used for the endpoints.
        /// </summary>
        private readonly ProtocolDataContractResolver m_DataContractResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpProtocolChannelTemplate"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <param name="dataContractResolver">The <see cref="DataContractResolver"/> that is used for the endpoints.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataContractResolver"/> is <see langword="null" />.
        /// </exception>
        public TcpProtocolChannelTemplate(IConfiguration tcpConfiguration, ProtocolDataContractResolver dataContractResolver)
            : base(tcpConfiguration)
        {
            {
                Lokad.Enforce.Argument(() => dataContractResolver);
            }

            m_DataContractResolver = dataContractResolver;
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
                    ReliableSession = new OptionalReliableSession
                        {
                            Enabled = true,
                            InactivityTimeout = Configuration.HasValueFor(
                                    CommunicationConfigurationKeys.BindingReceiveConfirmationTimeoutInMilliseconds)
                                ? TimeSpan.FromMilliseconds(
                                    Configuration.Value<int>(CommunicationConfigurationKeys.BindingReceiveConfirmationTimeoutInMilliseconds))
                                : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultBindingReceiveConfirmationTimeoutInMilliseconds),
                            Ordered = false,
                        },
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
            foreach (var operation in endpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = m_DataContractResolver;
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
            return Configuration.HasValueFor(CommunicationConfigurationKeys.TcpProtocolPath) ?
                Configuration.Value<string>(CommunicationConfigurationKeys.TcpProtocolPath) :
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
            // No need for a DataContractResolver on the data channel because there will not be any
            // unknown types
            var endpoint = host.AddServiceEndpoint(implementedContract, GenerateDataBinding(), GenerateNewDataAddress());
            return endpoint;
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
