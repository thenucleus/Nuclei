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
using System.Threading;
using System.Threading.Tasks;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the methods needed to communicate with one or more remote applications.
    /// </summary>
    internal sealed class CommunicationLayer : ICommunicationLayer
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection of endpoints that have been discovered.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_Endpoints;

        /// <summary>
        /// The collection of <see cref="IChannelTemplate"/> objects which refer to a communication.
        /// </summary>
        private readonly Dictionary<ChannelTemplate, Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_OpenConnections =
            new Dictionary<ChannelTemplate, Tuple<ICommunicationChannel, IDirectIncomingMessages>>();

        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();

        /// <summary>
        /// The collection of endpoint discovery objects.
        /// </summary>
        private readonly IEnumerable<IDiscoverOtherServices> m_DiscoverySources;

        /// <summary>
        /// The function that returns a tuple of a <see cref="ICommunicationChannel"/> and
        /// a <see cref="IDirectIncomingMessages"/> which belong together. The return values
        /// are based on the type of the <see cref="IChannelTemplate"/> for the channel.
        /// </summary>
        private readonly Func<ChannelTemplate, EndpointId, Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_ChannelBuilder;

        /// <summary>
        /// The collection containing the types of channel that should be opened.
        /// </summary>
        private readonly IEnumerable<ChannelTemplate> m_ChannelTypesToUse;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Indicates if the layer is signed on or not.
        /// </summary>
        private volatile bool m_AlreadySignedOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayer"/> class.
        /// </summary>
        /// <param name="endpoints">The collection that contains all the potential endpoints.</param>
        /// <param name="discoverySources">The object that handles the discovery of remote endpoints.</param>
        /// <param name="channelBuilder">
        ///     The function that returns a tuple of a <see cref="ICommunicationChannel"/> and a <see cref="IDirectIncomingMessages"/>
        ///     based on the type of the <see cref="IChannelTemplate"/> that is related to the channel.
        /// </param>
        /// <param name="channelTypesToUse">The collection that contains all the channel types for which a channel should be opened.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoints"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoverySources"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelTypesToUse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayer(
            IStoreInformationAboutEndpoints endpoints,
            IEnumerable<IDiscoverOtherServices> discoverySources,
            Func<ChannelTemplate, EndpointId, Tuple<ICommunicationChannel, IDirectIncomingMessages>> channelBuilder,
            IEnumerable<ChannelTemplate> channelTypesToUse,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => endpoints);
                Lokad.Enforce.Argument(() => discoverySources);
                Lokad.Enforce.Argument(() => channelBuilder);
                Lokad.Enforce.Argument(() => channelTypesToUse);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Endpoints = endpoints;
            m_ChannelBuilder = channelBuilder;
            m_DiscoverySources = discoverySources;
            m_ChannelTypesToUse = channelTypesToUse;
            m_Diagnostics = systemDiagnostics;

            m_Endpoints.OnEndpointConnected += HandleOnEndpointApproved;
            m_Endpoints.OnEndpointDisconnected += HandleEndpointDisconnected;
        }

        private void HandleOnEndpointApproved(object sender, EndpointEventArgs e)
        {
            RaiseOnEndpointConnected(e.Endpoint);
        }

        // How do we handle endpoints disappearing and then reappearing. If the remote process
        // crashed then we'll have an enpoint on the same machine but with another process id.
        // The catch is that we can't just look for the machine name because there is a possibility
        // that there will be more than one process on the machine .... Gah
        //
        // Also note that it is quite easily possible to fake being another endpoint. All you have
        // to do is send a message saying that you're a different endpoint and then the evil is
        // done. Not quite sure how to make that not happen though ...
        private void HandleEndpointDisconnected(object sender, EndpointEventArgs args)
        {
            if (m_Id.Equals(args.Endpoint))
            {
                return;
            }

            RaiseOnEndpointDisconnected(args.Endpoint);

            foreach (var pair in m_OpenConnections)
            {
                pair.Value.Item1.EndpointDisconnected(args.Endpoint);
                pair.Value.Item2.OnEndpointDisconnected(args.Endpoint);
            }
        }

        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        public EndpointId Id
        {
            [DebuggerStepThrough]
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has signed on with
        /// the network.
        /// </summary>
        public bool IsSignedIn
        {
            [DebuggerStepThrough]
            get
            {
                return m_AlreadySignedOn;
            }
        }

        /// <summary>
        /// Gets the connection information for the channel of a given type created by the current application.
        /// </summary>
        /// <param name="protocolVersion">The version of the protocol for which the connection information is required.</param>
        /// <param name="channelTemplate">The type of channel for which the connection information is required.</param>
        /// <returns>
        /// A tuple containing the <see cref="EndpointId"/>, the <see cref="Uri"/> of the message channel and the 
        /// <see cref="Uri"/> of the data channel; returns <see langword="null" /> if no channel of the given type exists.
        /// </returns>
        public Tuple<EndpointId, Uri, Uri> LocalConnectionFor(Version protocolVersion, ChannelTemplate channelTemplate)
        {
            Tuple<EndpointId, Uri, Uri> result = null;
            if (m_OpenConnections.ContainsKey(channelTemplate))
            {
                var connection = m_OpenConnections[channelTemplate].Item1.LocalConnectionPointForVersion(protocolVersion);
                if (connection == null)
                {
                    return null;
                }

                result = new Tuple<EndpointId, Uri, Uri>(m_Id, connection.MessageAddress, connection.DataAddress);
            }

            return result;
        }

        /// <summary>
        /// Returns a collection containing the endpoint IDs of the known remote endpoints.
        /// </summary>
        /// <returns>
        ///     The collection that contains the endpoint IDs of the remote endpoints.
        /// </returns>
        public IEnumerable<EndpointId> KnownEndpoints()
        {
            return new List<EndpointId>(m_Endpoints);
        }

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        public void SignIn()
        {
            if (m_AlreadySignedOn)
            {
                return;
            }

            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "CommunicationLayer: Signing in"))
            {
                // Always open our own channels so that the message processors and the handshake protocol
                // are all created and initialized
                foreach (var channelType in m_ChannelTypesToUse)
                {
                    OpenChannel(channelType);
                }

                // Initiate discovery of other services. 
                foreach (var source in m_DiscoverySources)
                {
                    source.StartDiscovery();
                }

                m_AlreadySignedOn = true;
            }

            m_Diagnostics.Log(LevelToLog.Trace, CommunicationConstants.DefaultLogTextPrefix, "Sign on process finished.");
            RaiseOnSignedIn();
        }

        /// <summary>
        /// Indicates if there is a channel for the given channel type.
        /// </summary>
        /// <param name="channelTemplate">The type of the channel.</param>
        /// <returns>
        /// <see langword="true"/> if there is a channel of the given type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private bool HasChannelFor(ChannelTemplate channelTemplate)
        {
            return m_OpenConnections.ContainsKey(channelTemplate);
        }

        /// <summary>
        /// Opens a channel of the given type.
        /// </summary>
        /// <param name="channelTemplate">The channel type to open.</param>
        /// <exception cref="InvalidChannelTypeException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see cref="ChannelTemplate.None"/>.
        /// </exception>
        private void OpenChannel(ChannelTemplate channelTemplate)
        {
            {
                Lokad.Enforce.With<InvalidChannelTypeException>(
                    channelTemplate != ChannelTemplate.None, 
                    Resources.Exceptions_Messages_AChannelTypeMustBeDefined);
            }

            if (HasChannelFor(channelTemplate))
            {
                return;
            }

            lock (m_Lock)
            {
                var pair = m_ChannelBuilder(channelTemplate, m_Id);
                m_OpenConnections.Add(channelTemplate, pair);
                pair.Item1.OpenChannel();
            }
        }

        private Tuple<ICommunicationChannel, IDirectIncomingMessages> ChannelInformationForType(ChannelTemplate connection)
        {
            Tuple<ICommunicationChannel, IDirectIncomingMessages> channel = null;
            lock (m_Lock)
            {
                if (m_OpenConnections.ContainsKey(connection))
                {
                    channel = m_OpenConnections[connection];
                }

                return channel;
            }
        }

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        public void SignOut()
        {
            if (!m_AlreadySignedOn)
            {
                return;
            }

            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "CommunicationLayer: signing out"))
            {
                // Stop discovering other services. We just stopped caring.
                foreach (var source in m_DiscoverySources)
                {
                    source.EndDiscovery();
                }

                // There may be a race condition here. We could be disconnecting while
                // others may be trying to connect, so we might have to put a 
                // lock in here to block things from happening.
                //
                // Disconnect from all channels
                lock (m_Lock)
                {
                    foreach (var pair in m_OpenConnections)
                    {
                        var connection = pair.Value.Item1;
                        connection.CloseChannel();
                    }

                    // Clear all connections
                    m_OpenConnections.Clear();
                }

                m_AlreadySignedOn = false;
            }

            m_Diagnostics.Log(LevelToLog.Trace, CommunicationConstants.DefaultLogTextPrefix, "Sign off process finished.");
            RaiseOnSignedOut();
        }

        /// <summary>
        /// An event raised when the layer has signed in.
        /// </summary>
        public event EventHandler<EventArgs> OnSignedIn;

        private void RaiseOnSignedIn()
        {
            var local = OnSignedIn;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the layer has signed out.
        /// </summary>
        public event EventHandler<EventArgs> OnSignedOut;

        private void RaiseOnSignedOut()
        {
            var local = OnSignedOut;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns a value indicating if the given endpoint has provided the information required to
        /// contact it if it isn't offline.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint has provided the information necessary to contact 
        ///     it over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsEndpointContactable(EndpointId endpoint)
        {
            return (endpoint != null) && m_Endpoints.CanCommunicateWithEndpoint(endpoint);
        }

        private EndpointInformation RetrieveEndpointConnection(EndpointId endpoint)
        {
            EndpointInformation information;
            m_Endpoints.TryGetConnectionFor(endpoint, out information);

            return information;
        }

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="EndpointNotContactableException">
        ///     Thrown if <paramref name="endpoint"/> has not provided any contact information.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void SendMessageTo(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.With<EndpointNotContactableException>(
                    m_Endpoints.CanCommunicateWithEndpoint(endpoint),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpoint);

                Lokad.Enforce.Argument(() => message);
            }

            using (m_Diagnostics.Profiler.Measure(
                CommunicationConstants.TimingGroup, 
                "CommunicationLayer: sending message without waiting for response"))
            {
                var connection = RetrieveEndpointConnection(endpoint);
                if (connection == null)
                {
                    throw new EndpointNotContactableException(endpoint);
                }

                var channel = ChannelFor(connection);
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Sending msg of type {0} to endpoint ({1}) without waiting for the response.",
                        message.GetType(),
                        endpoint));

                channel.Send(endpoint, message);
            }
        }

        private ICommunicationChannel ChannelFor(EndpointInformation connection)
        {
            return ChannelPairFor(connection).Item1;
        }

        private Tuple<ICommunicationChannel, IDirectIncomingMessages> ChannelPairFor(EndpointInformation connection)
        {
            var template = connection.ProtocolInformation.MessageAddress.ToChannelTemplate();
            Debug.Assert(
                template != ChannelTemplate.None && template != ChannelTemplate.Unknown,
                "The channel template should be a known type.");

            if (!HasChannelFor(template))
            {
                OpenChannel(template);
            }

            var pair = ChannelInformationForType(template);
            Debug.Assert(pair != null, "The channel should exist.");
            return pair;
        }

        /// <summary>
        /// Sends the given message to the specified endpoint and returns a task that
        /// will eventually contain the return message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <returns>A task object that will eventually contain the response message.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="EndpointNotContactableException">
        ///     Thrown if <paramref name="endpoint"/> has not provided any contact information.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public Task<ICommunicationMessage> SendMessageAndWaitForResponse(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.With<EndpointNotContactableException>(
                    m_Endpoints.CanCommunicateWithEndpoint(endpoint),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpoint);

                Lokad.Enforce.Argument(() => message);
            }

            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "CommunicationLayer: sending message and waiting for response"))
            {
                var connection = RetrieveEndpointConnection(endpoint);
                if (connection == null)
                {
                    throw new EndpointNotContactableException(endpoint);
                }

                var pair = ChannelPairFor(connection);
                var result = pair.Item2.ForwardResponse(endpoint, message.Id);

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Sending msg of type {0} to endpoint ({1}) while waiting for the response.",
                        message.GetType(),
                        endpoint));

                pair.Item1.Send(endpoint, message);
                return result;
            }
        }

        /// <summary>
        /// Uploads a given file to a specific endpoint.
        /// </summary>
        /// <param name="receivingEndpoint">The endpoint that will receive the data stream.</param>
        /// <param name="filePath">The full path to the file that should be transferred.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        ///     A task that will return once the upload is complete.
        /// </returns>
        public Task UploadData(
            EndpointId receivingEndpoint,
            string filePath,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Lokad.Enforce.Argument(() => filePath);
            }

            var connection = RetrieveEndpointConnection(receivingEndpoint);
            if (connection == null)
            {
                throw new EndpointNotContactableException(receivingEndpoint);
            }

            var channel = ChannelFor(connection);
            return channel.TransferData(receivingEndpoint, filePath, token, scheduler);
        }

        /// <summary>
        /// Returns a collection containing information describing all the active channels.
        /// </summary>
        /// <returns>The collection containing information describing all the active channels.</returns>
        public IEnumerable<ProtocolInformation> ActiveChannels()
        {
            var list = new List<ProtocolInformation>();
            lock (m_Lock)
            {
                list.AddRange(m_OpenConnections.Values.SelectMany(t => t.Item1.LocalConnectionPoints()));
            }

            return list;
        }

        /// <summary>
        /// An event raised when an endpoint has signed in.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointConnected;

        private void RaiseOnEndpointConnected(EndpointId id)
        {
            var local = OnEndpointConnected;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised when an endpoint has signed out.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointDisconnected;

        private void RaiseOnEndpointDisconnected(EndpointId endpoint)
        {
            var local = OnEndpointDisconnected;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SignOut();
        }
    }
}
