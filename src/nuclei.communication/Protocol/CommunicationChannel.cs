//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the methods required for handling communication with other Apollo applications 
    /// across the network.
    /// </summary>
    /// <remarks>
    /// The design of this class assumes that there is only one of these active for a given
    /// channel address (e.g. net.tcp://my_machine:7000/apollo) at any given time.
    /// This is because the current class has a receiving endpoint of which there can only 
    /// be one. If there are multiple communication channels sharing the receiving endpoint then
    /// we don't know which channel should get the messages.
    /// </remarks>
    internal sealed class CommunicationChannel : ICommunicationChannel, IDisposable
    {
        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// Maps the endpoint to the connection information.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_ChannelConnectionMap;

        /// <summary>
        /// Indicates the type of channel that we're dealing with and provides
        /// utility methods for the channel.
        /// </summary>
        private readonly IProtocolChannelTemplate m_Template;

        /// <summary>
        /// The host information for the message sending host.
        /// </summary>
        private readonly IHoldServiceConnections m_MessageHost;

        /// <summary>
        /// The host information for the data transferring host.
        /// </summary>
        private readonly IHoldServiceConnections m_DataHost;

        /// <summary>
        /// The function used to build message pipes.
        /// </summary>
        private readonly Func<IMessagePipe> m_MessageMessageReceiverBuilder;

        /// <summary>
        /// The function used to build data pipes.
        /// </summary>
        private readonly Func<IDataPipe> m_DataReceiverBuilder;

        /// <summary>
        /// The function that generates sending endpoints.
        /// </summary>
        private readonly BuildSendingEndpoint m_SenderBuilder;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The object used to send messages over the network.
        /// </summary>
        private ISendingEndpoint m_Sender;

        /// <summary>
        /// The receiving endpoint used to receive messages.
        /// </summary>
        private IMessagePipe m_MessageReceiver;

        /// <summary>
        /// The receiving endpoint used to receive data.
        /// </summary>
        private IDataPipe m_DataReceiver;

        /// <summary>
        /// The message handler that is used to receive messages from the
        /// receiving endpoint.
        /// </summary>
        private EventHandler<MessageEventArgs> m_MessageReceivingHandler;

        /// <summary>
        /// The event handler that is used to receive data from the receiving endpoint.
        /// </summary>
        private EventHandler<DataTransferEventArgs> m_DataReceivingHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationChannel"/> class.
        /// </summary>
        /// <param name="id">The ID number of the current endpoint.</param>
        /// <param name="connectionMap">The object that stores the connection information for the endpoints.</param>
        /// <param name="channelTemplate">The type of channel, e.g. TCP.</param>
        /// <param name="messageHost">
        /// The object that handles the <see cref="ServiceHost"/> for the channel used to send messages on.
        /// </param>
        /// <param name="dataHost">
        /// The object that handles the <see cref="ServiceHost"/> for the channel used to send data on.
        /// </param>
        /// <param name="messageReceiverBuilder">The function that builds message receiving endpoints.</param>
        /// <param name="dataReceiverBuilder">The function that builds data receiving endpoints.</param>
        /// <param name="senderBuilder">The function that builds sending endpoints.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connectionMap"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageHost"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataHost"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageReceiverBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataReceiverBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="senderBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public CommunicationChannel(
            EndpointId id, 
            IStoreInformationAboutEndpoints connectionMap,
            IProtocolChannelTemplate channelTemplate, 
            IHoldServiceConnections messageHost,
            IHoldServiceConnections dataHost,
            Func<IMessagePipe> messageReceiverBuilder,
            Func<IDataPipe> dataReceiverBuilder,
            BuildSendingEndpoint senderBuilder,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => connectionMap);
                Lokad.Enforce.Argument(() => channelTemplate);
                Lokad.Enforce.Argument(() => messageHost);
                Lokad.Enforce.Argument(() => dataHost);
                Lokad.Enforce.Argument(() => messageReceiverBuilder);
                Lokad.Enforce.Argument(() => dataReceiverBuilder);
                Lokad.Enforce.Argument(() => senderBuilder);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Id = id;
            m_ChannelConnectionMap = connectionMap;
            m_Template = channelTemplate;
            m_MessageHost = messageHost;
            m_DataHost = dataHost;

            m_MessageMessageReceiverBuilder = messageReceiverBuilder;
            m_DataReceiverBuilder = dataReceiverBuilder;
            m_SenderBuilder = senderBuilder;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Gets the connection information that describes the local endpoint.
        /// </summary>
        public ChannelConnectionInformation LocalConnectionPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        public void OpenChannel()
        {
            if ((m_MessageReceivingHandler != null) || (m_DataReceivingHandler != null))
            {
                CloseChannel();
            }

            m_DataReceivingHandler = (s, e) => RaiseOnDataReception(e.Data);
            m_DataReceiver = m_DataReceiverBuilder();
            m_DataReceiver.OnNewData += m_DataReceivingHandler;

            Func<ServiceHost, ServiceEndpoint> dataEndpointBuilder =
                host =>
                {
                    var dataEndpoint = m_Template.AttachDataEndpoint(host, typeof(IDataReceivingEndpoint));
                    return dataEndpoint;
                };
            var dataUri = m_DataHost.OpenChannel(m_DataReceiver, dataEndpointBuilder);

            m_MessageReceivingHandler = (s, e) => RaiseOnMessageReception(e.Message);
            m_MessageReceiver = m_MessageMessageReceiverBuilder();
            m_MessageReceiver.OnNewMessage += m_MessageReceivingHandler;

            Func<ServiceHost, ServiceEndpoint> messageEndpointBuilder = 
                host =>
                {
                    var messageEndpoint = m_Template.AttachMessageEndpoint(host, typeof(IMessageReceivingEndpoint), m_Id);
                    return messageEndpoint;
                };
            var messageUri = m_MessageHost.OpenChannel(m_MessageReceiver, messageEndpointBuilder);
            
            LocalConnectionPoint = new ChannelConnectionInformation(m_Id, m_Template.ChannelTemplate, messageUri, dataUri);
        }

        /// <summary>
        /// Closes the current channel.
        /// </summary>
        public void CloseChannel()
        {
            if (m_Sender != null)
            {
                // First notify the recipients that we're closing the channel.
                var knownEndpoints = new List<EndpointId>(m_Sender.KnownEndpoints());
                foreach (var key in knownEndpoints)
                {
                    var msg = new EndpointDisconnectMessage(m_Id);
                    try
                    {
                        m_Sender.Send(key, msg);
                    }
                    catch (FailedToSendMessageException)
                    {
                        // For some reason the message didn't arrive. Honestly we don't
                        // care, we're about to quit, not our problem anymore.
                    }
                }

                // Then close the channel. We'll do this in a different
                // loop to give the channels time to process the messages.
                foreach (var key in knownEndpoints)
                {
                    m_Sender.CloseChannelTo(key);
                }
            }

            CleanupHost();

            if (m_MessageReceiver != null)
            {
                m_MessageReceiver.OnNewMessage -= m_MessageReceivingHandler;
                m_MessageReceivingHandler = null;
                m_MessageReceiver = null;
            }

            if (m_DataReceiver != null)
            {
                m_DataReceiver.OnNewData -= m_DataReceivingHandler;
                m_DataReceivingHandler = null;
                m_DataReceiver = null;
            }

            LocalConnectionPoint = null;
            RaiseOnClosed();
        }

        private void CleanupHost()
        {
            m_MessageHost.CloseConnection();
            m_DataHost.CloseConnection();
        }

        /// <summary>
        /// Indicates that the remote endpoint has disconnected.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint that has disconnected.</param>
        public void EndpointDisconnected(EndpointId endpoint)
        {
            if (m_ChannelConnectionMap.CanCommunicateWithEndpoint(endpoint))
            {
                if (m_Sender != null)
                {
                    m_Sender.CloseChannelTo(endpoint);
                }
            }
        }

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="receivingEndpoint">The endpoint that will receive the data stream.</param>
        /// <param name="filePath">The file path to the file that should be transferred.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task with.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        public Task TransferData(
            EndpointId receivingEndpoint,
            string filePath,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Lokad.Enforce.Argument(() => filePath);
                Lokad.Enforce.Argument(() => filePath, Lokad.Rules.StringIs.NotEmpty);
            }

            if (m_Sender == null)
            {
                m_Sender = m_SenderBuilder(m_Id, BuildMessageSendingProxy, BuildDataTransferProxy);
            }

            return Task.Factory.StartNew(
                () =>
                {
                    // Don't catch any exception because the task will store them if we don't catch them.
                    using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        m_Sender.Send(receivingEndpoint, file);
                    }
                },
                token,
                TaskCreationOptions.LongRunning,
                scheduler ?? TaskScheduler.Default);
        }

        /// <summary>
        /// Sends the given message to the receiving endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message that should be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void Send(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => message);
            }

            if (m_Sender == null)
            {
                m_Sender = m_SenderBuilder(m_Id, BuildMessageSendingProxy, BuildDataTransferProxy);
            }

            m_Sender.Send(endpoint, message);
        }

        private IMessageSendingEndpoint BuildMessageSendingProxy(EndpointId id)
        {
            Debug.Assert(
                m_ChannelConnectionMap.CanCommunicateWithEndpoint(id) 
                    || m_ChannelConnectionMap.IsWaitingForApproval(id)
                    || m_ChannelConnectionMap.HasBeenContacted(id), 
                "Trying to send a message to an unknown endpoint.");
            ChannelConnectionInformation connectionInfo;
            var success = m_ChannelConnectionMap.TryGetConnectionFor(id, out connectionInfo);
            if (!success)
            {
                throw new EndpointNotContactableException();
            }

            var endpoint = new EndpointAddress(connectionInfo.MessageAddress);

            Debug.Assert(m_Template.ChannelTemplate == connectionInfo.ChannelTemplate, "Trying to connect to a channel with a different binding type.");
            var binding = m_Template.GenerateMessageBinding();

            var factory = new ChannelFactory<IMessageReceivingEndpointProxy>(binding, endpoint);
            return new RestoringMessageSendingEndpoint(factory, m_Diagnostics);
        }

        private IDataTransferingEndpoint BuildDataTransferProxy(EndpointId id)
        {
            Debug.Assert(
                m_ChannelConnectionMap.CanCommunicateWithEndpoint(id)
                    || m_ChannelConnectionMap.IsWaitingForApproval(id)
                    || m_ChannelConnectionMap.HasBeenContacted(id),
                "Trying to send data to an unknown endpoint.");
            ChannelConnectionInformation connectionInfo;
            var success = m_ChannelConnectionMap.TryGetConnectionFor(id, out connectionInfo);
            if (!success)
            {
                throw new EndpointNotContactableException();
            }

            var endpoint = new EndpointAddress(connectionInfo.DataAddress);

            Debug.Assert(m_Template.ChannelTemplate == connectionInfo.ChannelTemplate, "Trying to connect to a channel with a different binding type.");
            var binding = m_Template.GenerateDataBinding();

            var factory = new ChannelFactory<IDataReceivingEndpointProxy>(binding, endpoint);
            return new RestoringDataTransferingEndpoint(factory, m_Diagnostics);
        }

        /// <summary>
        /// An event raised when a new message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessageReception;

        private void RaiseOnMessageReception(ICommunicationMessage message)
        {
            var local = OnMessageReception;
            if (local != null)
            {
                local(this, new MessageEventArgs(message));
            }
        }

        /// <summary>
        /// An event raised when a new data stream is received.
        /// </summary>
        public event EventHandler<DataTransferEventArgs> OnDataReception;

        private void RaiseOnDataReception(DataTransferMessage message)
        {
            var local = OnDataReception;
            if (local != null)
            {
                local(this, new DataTransferEventArgs(message));
            }
        }

        /// <summary>
        /// An event raised when the the channel is closed.
        /// </summary>
        public event EventHandler<ChannelClosedEventArgs> OnClosed;

        private void RaiseOnClosed()
        {
            var local = OnClosed;
            if (local != null)
            {
                local(this, new ChannelClosedEventArgs(m_Id));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CloseChannel();
        }
    }
}
