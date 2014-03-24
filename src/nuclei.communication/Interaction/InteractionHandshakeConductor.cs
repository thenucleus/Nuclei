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
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the handshake behavior for the interaction layer.
    /// </summary>
    internal sealed class InteractionHandshakeConductor : IHandleInteractionHandshakes
    {
        /// <summary>
        /// Stores information about the messages that have been send and received.
        /// </summary>
        private sealed class EndpointApprovalState
        {
            /// <summary>
            /// The collection containing the commands that were selected for use
            /// for the current remote endpoint.
            /// </summary>
            private readonly List<OfflineTypeInformation> m_SelectedCommands
                = new List<OfflineTypeInformation>();

            /// <summary>
            /// The collection containing the notifications that were selected for use
            /// for the current remote endpoint.
            /// </summary>
            private readonly List<OfflineTypeInformation> m_SelectedNotifications
                = new List<OfflineTypeInformation>();

            /// <summary>
            /// Initializes a new instance of the <see cref="EndpointApprovalState"/> class.
            /// </summary>
            public EndpointApprovalState()
            {
                SendResponse = InteractionConnectionState.None;
                ReceivedResponse = InteractionConnectionState.None;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application has send an
            /// <see cref="EndpointInteractionInformationMessage"/>.
            /// </summary>
            public bool HaveSendSubjects
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application has received an
            /// <see cref="EndpointInteractionInformationMessage"/>.
            /// </summary>
            public bool HaveReceivedSubjects
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the current application will want to maintain
            /// the connection to the remote endpoint.
            /// </summary>
            public InteractionConnectionState SendResponse
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the remote endpoint will want to maintain
            /// the connection to the current endpoint.
            /// </summary>
            public InteractionConnectionState ReceivedResponse
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the collection containing the commands that were selected for use
            /// for the current remote endpoint.
            /// </summary>
            public List<OfflineTypeInformation> SelectedCommands
            {
                get
                {
                    return m_SelectedCommands;
                }
            }

            /// <summary>
            /// Gets the collection containing the notifications that were selected for use
            /// for the current remote endpoint.
            /// </summary>
            public List<OfflineTypeInformation> SelectedNotifications
            {
                get
                {
                    return m_SelectedNotifications;
                }
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
                return HaveSendSubjects && HaveReceivedSubjects
                    && (SendResponse != InteractionConnectionState.None) && (ReceivedResponse != InteractionConnectionState.None);
            }
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that tracks which messages have been send and received from remote endpoints.
        /// </summary>
        private readonly Dictionary<EndpointId, EndpointApprovalState> m_EndpointApprovalState
            = new Dictionary<EndpointId, EndpointApprovalState>();

        /// <summary>
        /// The object that stores information about the known endpoints.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_EndpointInformationStorage;

        /// <summary>
        /// The collection that contains all the registered subjects and their commands and notifications.
        /// </summary>
        private readonly IStoreInteractionSubjects m_InteractionSubjects;

        /// <summary>
        /// The object that stores the command proxy objects.
        /// </summary>
        private readonly IStoreRemoteCommandProxies m_CommandProxyHub;

        /// <summary>
        /// The object that stores the notification proxy objects.
        /// </summary>
        private readonly IStoreRemoteNotificationProxies m_NotificationProxyHub;

        /// <summary>
        /// The object responsible for sending messages to remote endpoints.
        /// </summary>
        private readonly IProtocolLayer m_Layer;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionHandshakeConductor"/> class.
        /// </summary>
        /// <param name="endpointInformationStorage">The object that stores information about the known endpoints.</param>
        /// <param name="interactionSubjects">The collection that contains all the registered subjects and their commands and notifications.</param>
        /// <param name="commandProxyHub">The object that stores the command proxy objects.</param>
        /// <param name="notificationProxyHub">The object that stores the notification proxy objects.</param>
        /// <param name="layer">The object responsible for sending messages to remote endpoints.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointInformationStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="interactionSubjects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandProxyHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationProxyHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public InteractionHandshakeConductor(
            IStoreInformationAboutEndpoints endpointInformationStorage,
            IStoreInteractionSubjects interactionSubjects,
            IStoreRemoteCommandProxies commandProxyHub,
            IStoreRemoteNotificationProxies notificationProxyHub,
            IProtocolLayer layer,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => endpointInformationStorage);
                Lokad.Enforce.Argument(() => interactionSubjects);
                Lokad.Enforce.Argument(() => commandProxyHub);
                Lokad.Enforce.Argument(() => notificationProxyHub);
                Lokad.Enforce.Argument(() => layer);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_EndpointInformationStorage = endpointInformationStorage;
            m_EndpointInformationStorage.OnEndpointConnected += HandleEndpointSignIn;

            m_InteractionSubjects = interactionSubjects;
            m_CommandProxyHub = commandProxyHub;
            m_NotificationProxyHub = notificationProxyHub;
            m_Layer = layer;
            m_Diagnostics = diagnostics;
        }

        private void HandleEndpointSignIn(object sender, EndpointEventArgs e)
        {
            lock (m_Lock)
            {
                // Potentially a new endpoint so store it
                StorePotentialEndpoint(e.Endpoint);
                InitiateHandshakeWith(e.Endpoint);
            }
        }

        private void InitiateHandshakeWith(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (!m_EndpointApprovalState.ContainsKey(endpoint))
                {
                    return;
                }

                var interactionInformation = new List<CommunicationSubjectGroup>();
                foreach (var subject in m_InteractionSubjects.ProvidedSubjects())
                {
                    if (m_InteractionSubjects.ContainsGroupProvisionsForSubject(subject))
                    {
                        interactionInformation.Add(m_InteractionSubjects.GroupProvisionsFor(subject));
                    }
                }

                Debug.Assert(m_EndpointApprovalState.ContainsKey(endpoint), "The endpoint tick list is not stored.");
                var tickList = m_EndpointApprovalState[endpoint];
                tickList.HaveSendSubjects = true;

                // Send message and wait for response.
                var message = new EndpointInteractionInformationMessage(
                    m_Layer.Id,
                    interactionInformation.ToArray());
                var sendTask = m_Layer.SendMessageAndWaitForResponse(endpoint, message);
                sendTask.ContinueWith(HandleResponseToInteractionMessage, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void HandleResponseToInteractionMessage(Task<ICommunicationMessage> t)
        {
            var msg = t.Result;
            if (msg is FailureMessage)
            {
                RemoveEndpoint(msg.Sender);
                return;
            }

            var responseMessage = msg as EndpointInteractionInformationResponseMessage;
            if (responseMessage != null)
            {
                lock (m_Lock)
                {
                    Debug.Assert(m_EndpointApprovalState.ContainsKey(msg.Sender), "The endpoint tick list is not stored.");
                    m_EndpointApprovalState[msg.Sender].ReceivedResponse = responseMessage.State;

                    if (m_EndpointApprovalState[msg.Sender].IsComplete())
                    {
                        CloseOrApproveConnection(msg.Sender);
                    }
                }
            }
        }

        private void RemoveEndpoint(EndpointId sender)
        {
            if (m_EndpointApprovalState.ContainsKey(sender))
            {
                m_EndpointApprovalState.Remove(sender);
            }

            m_EndpointInformationStorage.TryRemoveEndpoint(sender);
        }

        /// <summary>
        /// Continues the handshake process between the current endpoint and the specified endpoint.
        /// </summary>
        /// <param name="connection">The ID of the endpoint that started the handshake.</param>
        /// <param name="subjectGroups">The handshake information for the endpoint.</param>
        /// <param name="messageId">The ID of the message that carried the handshake information.</param>
        public void ContinueHandshakeWith(EndpointId connection, CommunicationSubjectGroup[] subjectGroups, MessageId messageId)
        {
            bool shouldSendConnect;
            lock (m_Lock)
            {
                StorePotentialEndpoint(connection);
                if (!m_EndpointApprovalState.ContainsKey(connection))
                {
                    return;
                }

                var tickList = m_EndpointApprovalState[connection];
                tickList.HaveReceivedSubjects = true;

                bool foundAtLeastOneSubjectMatch = false;
                foreach (var subject in m_InteractionSubjects.RequiredSubjects())
                {
                    if (!m_InteractionSubjects.ContainsGroupRequirementsForSubject(subject))
                    {
                        continue;
                    }

                    var providedGroup = subjectGroups.FirstOrDefault(s => s.Subject.Equals(subject));
                    if (providedGroup == null)
                    {
                        continue;
                    }

                    var haveRequiredTypesForSubject = true;
                    var commands = new List<OfflineTypeInformation>();

                    var requiredGroup = m_InteractionSubjects.GroupRequirementsFor(subject);
                    foreach (var requiredCommandType in requiredGroup.Commands)
                    {
                        var providedCommand = providedGroup.Commands.FirstOrDefault(c => requiredCommandType.IsPartialMatch(c));
                        if (providedCommand == null)
                        {
                            haveRequiredTypesForSubject = false;
                            break;
                        }

                        var bestMatch = requiredCommandType.HighestVersionMatch(providedCommand);
                        commands.Add(bestMatch);
                    }

                    var notifications = new List<OfflineTypeInformation>();
                    if (haveRequiredTypesForSubject)
                    {
                        foreach (var requiredNotificationType in requiredGroup.Notifications)
                        {
                            var providedNotification = providedGroup.Notifications.FirstOrDefault(n => requiredNotificationType.IsPartialMatch(n));
                            if (providedNotification == null)
                            {
                                haveRequiredTypesForSubject = false;
                                break;
                            }

                            var bestMatch = requiredNotificationType.HighestVersionMatch(providedNotification);
                            notifications.Add(bestMatch);
                        }
                    }

                    if (haveRequiredTypesForSubject)
                    {
                        tickList.SelectedCommands.AddRange(commands);
                        tickList.SelectedNotifications.AddRange(notifications);

                        foundAtLeastOneSubjectMatch = true;
                    }
                }

                var state = foundAtLeastOneSubjectMatch ? InteractionConnectionState.Desired : InteractionConnectionState.Neutral;
                var message = new EndpointInteractionInformationResponseMessage(
                    m_Layer.Id,
                    messageId,
                    state);
                m_Layer.SendMessageTo(connection, message);
                tickList.SendResponse = state;

                if (tickList.IsComplete())
                {
                    CloseOrApproveConnection(connection);
                }

                shouldSendConnect = !tickList.HaveSendSubjects;
            }

            if (shouldSendConnect)
            {
                InitiateHandshakeWith(connection);
            }
        }

        private void StorePotentialEndpoint(EndpointId id)
        {
            lock (m_Lock)
            {
                if (!m_EndpointApprovalState.ContainsKey(id))
                {
                    m_EndpointApprovalState.Add(id, new EndpointApprovalState());

                    m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "New endpoint ({0}) provided for interaction handshake.",
                        id));
                }
            }
        }

        private void CloseOrApproveConnection(EndpointId connection)
        {
            lock (m_Lock)
            {
                if (!m_EndpointApprovalState.ContainsKey(connection))
                {
                    return;
                }

                var tickList = m_EndpointApprovalState[connection];
                if ((tickList.ReceivedResponse == InteractionConnectionState.Denied) 
                    || (tickList.SendResponse == InteractionConnectionState.Denied))
                {
                    RemoveEndpoint(connection);

                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Connection to endpoint ({0}) denied at interaction level.",
                            connection));
                }

                if ((tickList.ReceivedResponse == InteractionConnectionState.Desired) 
                    || (tickList.SendResponse == InteractionConnectionState.Desired))
                {
                    try
                    {
                        m_CommandProxyHub.OnReceiptOfEndpointCommands(connection, tickList.SelectedCommands);
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to store commands for endpoint ({0}). Error was {1}",
                                connection,
                                e));
                    }

                    try
                    {
                        m_NotificationProxyHub.OnReceiptOfEndpointNotifications(connection, tickList.SelectedNotifications);
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to store notifications for endpoint ({0}). Error was {1}",
                                connection,
                                e));
                    }

                    m_EndpointApprovalState.Remove(connection);

                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Connection to endpoint ({0}) approved at interaction level.",
                            connection));
                }
                else
                {
                    RemoveEndpoint(connection);

                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Connection to endpoint ({0}) not desired by either end at interaction level.",
                            connection));
                }
            }
        }
    }
}
