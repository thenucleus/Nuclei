//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the handshake behavior for the protocol layer.
    /// </summary>
    internal sealed class ProtocolHandshakeConductor : IHandleProtocolHandshakes
    {
        /// <summary>
        /// Stores information about the messages that have been send and received.
        /// </summary>
        private sealed class EndpointApprovalState
        {
            /// <summary>
            /// Gets or sets a value indicating whether the current application has send an
            /// <see cref="EndpointConnectMessage"/>.
            /// </summary>
            public bool HaveSendConnect
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application has received an
            /// <see cref="EndpointConnectMessage"/>.
            /// </summary>
            public bool HaveReceivedConnect
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application has send
            /// a response to the received <see cref="EndpointConnectMessage"/>.
            /// </summary>
            public bool HaveSendConnectResponse
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application has received a
            /// a response to the send <see cref="EndpointConnectMessage"/>.
            /// </summary>
            public bool HaveReceivedConnectResponse
            {
                get;
                set;
            }

            /// <summary>
            /// Indicates that all expected messages have been send and received.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> if all expected messages have been send and received; otherwise, <see langword="false" />.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsComplete()
            {
                return HaveSendConnect && HaveReceivedConnect && HaveSendConnectResponse && HaveReceivedConnectResponse;
            }
        }

        private static bool CanReachEndpointWithGivenConnection(EndpointInformation info)
        {
            var endpointChannelTemplate = info.ProtocolInformation.MessageAddress.ToChannelTemplate();
            if (info.Id.IsOnLocalMachine())
            {
                return endpointChannelTemplate == ChannelTemplate.NamedPipe;
            }

            return endpointChannelTemplate == ChannelTemplate.TcpIP;
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that contains the approvers for all supported protocol versions.
        /// </summary>
        private readonly Dictionary<Version, IApproveEndpointConnections> m_ConnectionApprovers
            = new Dictionary<Version, IApproveEndpointConnections>();

        /// <summary>
        /// The collection that tracks which messages have been send and received from remote endpoints.
        /// </summary>
        private readonly Dictionary<EndpointId, EndpointApprovalState> m_EndpointApprovalState
            = new Dictionary<EndpointId, EndpointApprovalState>();

        /// <summary>
        /// The collection of endpoints that have been discovered.
        /// </summary>
        private readonly IStoreEndpointApprovalState m_PotentialEndpoints;

        /// <summary>
        /// Provides the information describing the entry discovery channel.
        /// </summary>
        private readonly IProvideLocalConnectionInformation m_DiscoveryChannel;

        /// <summary>
        /// The collection of endpoint discovery objects.
        /// </summary>
        private readonly IEnumerable<IDiscoverOtherServices> m_DiscoverySources;

        /// <summary>
        /// The object responsible for sending messages to remote endpoints.
        /// </summary>
        private readonly IProtocolLayer m_Layer;

        /// <summary>
        /// The object that stores information about the available communication APIs.
        /// </summary>
        private readonly IStoreProtocolSubjects m_Descriptions;

        /// <summary>
        /// The collection containing the types of channel that should be opened.
        /// </summary>
        private readonly IEnumerable<ChannelTemplate> m_AllowedChannelTypes;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolHandshakeConductor"/> class.
        /// </summary>
        /// <param name="potentialEndpoints">The collection of endpoints that have been discovered.</param>
        /// <param name="discoveryChannel">The object that provides the information about the entry discovery channel for the application.</param>
        /// <param name="discoverySources">The object that handles the discovery of remote endpoints.</param>
        /// <param name="layer">The object responsible for sending messages with remote endpoints.</param>
        /// <param name="descriptions">The object that stores information about the available communication descriptions.</param>
        /// <param name="connectionApprovers">
        /// The collection that contains all the objects that are able to approve connections between the current endpoint and a remote endpoint.
        /// </param>
        /// <param name="allowedChannelTypes">The collection that contains all the channel types for which a channel should be opened.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="potentialEndpoints"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoveryChannel"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoverySources"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="descriptions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connectionApprovers"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="allowedChannelTypes"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public ProtocolHandshakeConductor(
            IStoreEndpointApprovalState potentialEndpoints,
            IProvideLocalConnectionInformation discoveryChannel,
            IEnumerable<IDiscoverOtherServices> discoverySources,
            IProtocolLayer layer,
            IStoreProtocolSubjects descriptions,
            IEnumerable<IApproveEndpointConnections> connectionApprovers,
            IEnumerable<ChannelTemplate> allowedChannelTypes,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => potentialEndpoints);
                Lokad.Enforce.Argument(() => discoveryChannel);
                Lokad.Enforce.Argument(() => discoverySources);
                Lokad.Enforce.Argument(() => layer);
                Lokad.Enforce.Argument(() => descriptions);
                Lokad.Enforce.Argument(() => connectionApprovers);
                Lokad.Enforce.Argument(() => allowedChannelTypes);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_PotentialEndpoints = potentialEndpoints;

            // Also handle the case where the endpoint signs out while it is trying to 
            // connect to us
            m_PotentialEndpoints.OnEndpointDisconnected += HandleEndpointSignedOut;

            m_DiscoveryChannel = discoveryChannel;
            m_DiscoverySources = discoverySources;
            m_Layer = layer;
            m_Descriptions = descriptions;
            m_AllowedChannelTypes = allowedChannelTypes;
            m_Diagnostics = systemDiagnostics;

            foreach (var approver in connectionApprovers)
            {
                m_ConnectionApprovers.Add(approver.ProtocolVersion, approver);
            }

            foreach (var source in m_DiscoverySources)
            {
                source.OnEndpointBecomingAvailable += HandleEndpointSignIn;
                source.OnEndpointBecomingUnavailable += HandleEndpointSignedOut;
            }
        }

        private void HandleEndpointSignIn(object sender, EndpointDiscoveredEventArgs args)
        {
            var info = args.ConnectionInformation;
            if (m_Layer.Id.Equals(info.Id))
            {
                return;
            }

            // Filter out the endpoint connections that can't be used (i.e. named pipes on remote machines
            // or TCP/IP connections on the local machine).
            if (!CanReachEndpointWithGivenConnection(info))
            {
                return;
            }

            // Filter out the endpoint connections that have a channel type that is not being used.
            if (!IsAllowedToCommunicateWithConnection(info))
            {
                return;
            }

            if (!m_PotentialEndpoints.CanCommunicateWithEndpoint(info.Id)
                && !m_PotentialEndpoints.IsWaitingForApproval(info.Id)
                && !m_PotentialEndpoints.HasBeenContacted(info.Id))
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "New endpoint ({0}) discovered at endpoint URL: {1}.",
                        info.Id,
                        info.ProtocolInformation.MessageAddress));

                StorePotentialEndpoint(info);
                InitiateHandshakeWith(info);
            }
        }

        private bool IsAllowedToCommunicateWithConnection(EndpointInformation info)
        {
            return m_AllowedChannelTypes.Contains(info.ProtocolInformation.MessageAddress.ToChannelTemplate());
        }

        // How do we handle endpoints disappearing and then reappearing. If the remote process
        // crashed then we'll have an enpoint on the same machine but with another process id.
        // The catch is that we can't just look for the machine name because there is a possibility
        // that there will be more than one process on the machine .... Gah
        //
        // Also note that it is quite easily possible to fake being another endpoint. All you have
        // to do is send a message saying that you're a different endpoint and then the evil is
        // done. Not quite sure how to make that not happen though ...
        private void HandleEndpointSignedOut(object sender, EndpointEventArgs args)
        {
            if (m_Layer.Id.Equals(args.Endpoint))
            {
                return;
            }

            RemoveEndpoint(args.Endpoint);
        }

        private void RemoveEndpoint(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                m_PotentialEndpoints.TryRemoveEndpoint(endpoint);
                if (m_EndpointApprovalState.ContainsKey(endpoint))
                {
                    m_EndpointApprovalState.Remove(endpoint);
                }
            }
        }

        /// <summary>
        /// Initiates the handshake process between the current endpoint and the specified endpoint.
        /// </summary>
        /// <param name="information">The connection information for the endpoint.</param>
        private void InitiateHandshakeWith(EndpointInformation information)
        {
            var connectionInfo = m_Layer.LocalConnectionFor(
                information.ProtocolInformation.Version, 
                information.ProtocolInformation.MessageAddress.ToChannelTemplate());
            if (connectionInfo != null)
            {
                lock (m_Lock)
                {
                    Debug.Assert(m_EndpointApprovalState.ContainsKey(information.Id), "The endpoint tick list is not stored.");
                    var tickList = m_EndpointApprovalState[information.Id];

                    if (!tickList.HaveSendConnect && !m_PotentialEndpoints.CanCommunicateWithEndpoint(information.Id))
                    {
                        tickList.HaveSendConnect = true;

                        var message = new EndpointConnectMessage(
                            m_Layer.Id,
                            new DiscoveryInformation(m_DiscoveryChannel.EntryChannel), 
                            new ProtocolInformation(
                                information.ProtocolInformation.Version,
                                connectionInfo.Item2,
                                connectionInfo.Item3), 
                            m_Descriptions.ToStorage());
                        var task = m_Layer.SendMessageAndWaitForResponse(information.Id, message);
                        task.ContinueWith(HandleResponseToConnectMessage, TaskContinuationOptions.ExecuteSynchronously);
                    }
                }
            }
        }

        private void HandleResponseToConnectMessage(Task<ICommunicationMessage> t)
        {
            var msg = t.Result;
            if (msg is FailureMessage)
            {
                RemoveEndpoint(msg.Sender);
                return;
            }

            if (msg is SuccessMessage)
            {
                lock (m_Lock)
                {
                    Debug.Assert(m_EndpointApprovalState.ContainsKey(msg.Sender), "The endpoint tick list is not stored.");
                    m_EndpointApprovalState[msg.Sender].HaveReceivedConnectResponse = true;

                    if (m_EndpointApprovalState[msg.Sender].IsComplete())
                    {
                        ApproveConnection(msg.Sender);
                    }
                }
            }
        }

        private void StorePotentialEndpoint(EndpointInformation information)
        {
            lock (m_Lock)
            {
                if (!m_EndpointApprovalState.ContainsKey(information.Id))
                {
                    m_EndpointApprovalState.Add(information.Id, new EndpointApprovalState());
                }

                var isStored = m_PotentialEndpoints.CanCommunicateWithEndpoint(information.Id)
                    || m_PotentialEndpoints.IsWaitingForApproval(information.Id)
                    || m_PotentialEndpoints.HasBeenContacted(information.Id);
                if (!isStored)
                {
                    m_PotentialEndpoints.TryAdd(information.Id, information);

                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "New endpoint {0} connected via {1}.",
                            information.Id,
                            information.ProtocolInformation.MessageAddress));
                }
            }
        }

        /// <summary>
        /// Continues the handshake process between the current endpoint and the specified endpoint.
        /// </summary>
        /// <param name="connection">The connection information for endpoint that started the handshake.</param>
        /// <param name="information">The handshake information for the endpoint.</param>
        /// <param name="messageId">The ID of the message that carried the handshake information.</param>
        public void ContinueHandshakeWith(
            EndpointInformation connection,
            ProtocolDescription information,
            MessageId messageId)
        {
            bool shouldSendConnect;
            lock (m_Lock)
            {
                // Potentially a new endpoint so store it
                StorePotentialEndpoint(connection);
                if (!m_EndpointApprovalState.ContainsKey(connection.Id))
                {
                    return;
                }

                var tickList = m_EndpointApprovalState[connection.Id];
                tickList.HaveReceivedConnect = true;

                if (!AllowConnection(connection.ProtocolInformation.Version, information))
                {
                    var failMsg = new FailureMessage(m_Layer.Id, messageId);
                    m_Layer.SendMessageTo(connection.Id, failMsg);

                    RemoveEndpoint(connection.Id);
                    return;
                }

                var successMessage = new SuccessMessage(m_Layer.Id, messageId);
                m_Layer.SendMessageTo(connection.Id, successMessage);
                tickList.HaveSendConnectResponse = true;

                m_PotentialEndpoints.TryStartApproval(connection.Id, information);
                if (tickList.IsComplete())
                {
                    ApproveConnection(connection.Id);
                }

                shouldSendConnect = !tickList.HaveSendConnect;
            }

            if (shouldSendConnect)
            {
                InitiateHandshakeWith(connection);
            }
        }

        private void ApproveConnection(EndpointId connection)
        {
            if (m_PotentialEndpoints.IsWaitingForApproval(connection))
            {
                EndpointInformation info;
                var connectionSuccess = m_PotentialEndpoints.TryGetConnectionFor(connection, out info);
                if (!connectionSuccess)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to get connection information for endpoint: {0}",
                            connection));

                    return;
                }

                if (m_PotentialEndpoints.TryCompleteApproval(connection))
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "New endpoint ({0}) approved for communication. Message URL: {1}. Data URL: {2}",
                            info.Id,
                            info.ProtocolInformation.MessageAddress,
                            info.ProtocolInformation.DataAddress));
                }
            }

            if (m_EndpointApprovalState.ContainsKey(connection))
            {
                m_EndpointApprovalState.Remove(connection);
            }
        }

        private bool AllowConnection(Version requiredProtocolVersion, ProtocolDescription information)
        {
            if (!m_ConnectionApprovers.ContainsKey(requiredProtocolVersion))
            {
                return false;
            }

            var approver = m_ConnectionApprovers[requiredProtocolVersion];
            return approver.IsEndpointAllowedToConnect(information);
        }
    }
}
