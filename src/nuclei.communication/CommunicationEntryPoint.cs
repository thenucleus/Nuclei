//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Communication.Interaction;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the main user entry point for the communication system.
    /// </summary>
    internal sealed class CommunicationEntryPoint : ICommunicationFacade
    {
        /// <summary>
        /// Maps the 'progress' of an endpoint through the connection approval process.
        /// </summary>
        private sealed class EndpointConnectionCompletionMap
        {
            /// <summary>
            /// Gets or sets a value indicating whether the endpoint has been approved for
            /// communication.
            /// </summary>
            public bool HasBeenApprovedForProtocol
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the endpoint has been approved
            /// for use of commands.
            /// </summary>
            public bool HasBeenApprovedForCommands
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the endpoint has been approved
            /// for use of notifications.
            /// </summary>
            public bool HasBeenApprovedForNotifications
            {
                get;
                set;
            }
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The endpoint ID for the current process.
        /// </summary>
        private readonly EndpointId m_LocalEndpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();

        /// <summary>
        /// The collection that contains the completion maps for the endpoints that are in the process of connecting.
        /// </summary>
        private readonly Dictionary<EndpointId, EndpointConnectionCompletionMap> m_EndpointConnectionProgress
            = new Dictionary<EndpointId, EndpointConnectionCompletionMap>();

        /// <summary>
        /// The collection that maps a known endpoint to its discovery URI.
        /// </summary>
        private readonly Dictionary<EndpointId, Uri> m_EndpointMap
            = new Dictionary<EndpointId, Uri>();

        /// <summary>
        /// The collection that maps a discovery URI to a known endpoint.
        /// </summary>
        private readonly Dictionary<Uri, EndpointId> m_UriMap
            = new Dictionary<Uri, EndpointId>();

        /// <summary>
        /// The object that stores information about all known endpoints.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_EndpointStorage;

        /// <summary>
        /// The collection that stores the commands for all known endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Commands;

        /// <summary>
        /// The collection that stores notifications for all known endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_Notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationEntryPoint"/> class.
        /// </summary>
        /// <param name="endpointStorage">The collection that stores information about all known endpoints.</param>
        /// <param name="commands">The collection that stores information about all the known remote commands.</param>
        /// <param name="notifications">The collection that stores information about all the known remote notifications.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        public CommunicationEntryPoint(
            IStoreInformationAboutEndpoints endpointStorage,
            ISendCommandsToRemoteEndpoints commands,
            INotifyOfRemoteEndpointEvents notifications)
        {
            {
                Lokad.Enforce.Argument(() => endpointStorage);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
            }

            m_EndpointStorage = endpointStorage;
            m_EndpointStorage.OnEndpointConnected += HandleOnEndpointConnected;
            m_EndpointStorage.OnEndpointDisconnected += HandleOnEndpointDisconnected;

            m_Commands = commands;
            m_Commands.OnEndpointConnected += HandleOnEndpointConnected;
            m_Commands.OnEndpointDisconnected += HandleOnEndpointDisconnected;

            m_Notifications = notifications;
            m_Notifications.OnEndpointConnected += HandleOnEndpointConnected;
            m_Notifications.OnEndpointDisconnected += HandleOnEndpointDisconnected;
        }

        private void HandleOnEndpointConnected(object sender, EndpointEventArgs e)
        {
            var hasEndpointBeenApproved = false;

            var id = e.Endpoint;
            lock (m_Lock)
            {
                if (!m_EndpointConnectionProgress.ContainsKey(id))
                {
                    m_EndpointConnectionProgress.Add(id, new EndpointConnectionCompletionMap());
                }

                var map = m_EndpointConnectionProgress[id];
                map.HasBeenApprovedForProtocol = map.HasBeenApprovedForProtocol || ReferenceEquals(sender, m_EndpointStorage);
                map.HasBeenApprovedForCommands = map.HasBeenApprovedForCommands || ReferenceEquals(sender, m_Commands);
                map.HasBeenApprovedForNotifications = map.HasBeenApprovedForNotifications || ReferenceEquals(sender, m_Notifications);

                if (map.HasBeenApprovedForProtocol && map.HasBeenApprovedForCommands && map.HasBeenApprovedForNotifications)
                {
                    EndpointInformation information;
                    if (!m_EndpointStorage.TryGetConnectionFor(id, out information))
                    {
                        return;
                    }

                    if (!m_EndpointMap.ContainsKey(id))
                    {
                        m_EndpointMap.Add(id, information.DiscoveryInformation.Address);
                    }

                    if (m_UriMap.ContainsKey(information.DiscoveryInformation.Address))
                    {
                        m_UriMap.Add(information.DiscoveryInformation.Address, id);
                    }

                    m_EndpointConnectionProgress.Remove(id);
                    hasEndpointBeenApproved = true;
                }
            }

            if (hasEndpointBeenApproved)
            {
                RaiseOnEndpointConnected(id);
            }
        }

        private void HandleOnEndpointDisconnected(object sender, EndpointEventArgs e)
        {
            var id = e.Endpoint;
            lock (m_Lock)
            {
                if (m_EndpointConnectionProgress.ContainsKey(id))
                {
                    m_EndpointConnectionProgress.Remove(id);
                }

                Uri matchingUri = null;
                if (m_EndpointMap.ContainsKey(id))
                {
                    matchingUri = m_EndpointMap[id];
                    m_EndpointMap.Remove(id);
                }

                if ((matchingUri != null) && m_UriMap.ContainsKey(matchingUri))
                {
                    m_UriMap.Remove(matchingUri);
                }
            }

            RaiseOnEndpointDisconnected(id);
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

        private void RaiseOnEndpointDisconnected(EndpointId id)
        {
            var local = OnEndpointDisconnected;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
            }
        }

        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        public EndpointId Id
        {
            get
            {
                return m_LocalEndpoint;
            }
        }

        /// <summary>
        /// Returns a collection containing all the known remote endpoints.
        /// </summary>
        /// <returns>The collection of all the known remote endpoints.</returns>
        public IEnumerable<EndpointId> KnownEndpoints()
        {
            return m_EndpointMap.Keys;
        }

        /// <summary>
        /// Gets the endpoint ID for the endpoint with its discovery channel at the given URI.
        /// </summary>
        /// <param name="address">The URI of the discovery channel.</param>
        /// <returns>The endpoint ID of the endpoint with a discovery channel at the given URI.</returns>
        public EndpointId FromUri(Uri address)
        {
            if (!m_UriMap.ContainsKey(address))
            {
                return null;
            }

            return m_UriMap[address];
        }
    }
}
