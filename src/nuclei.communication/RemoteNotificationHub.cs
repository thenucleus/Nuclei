//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the methods for handling communication notifications.
    /// </summary>
    /// <remarks>
    /// Objects can register <see cref="INotificationSet"/> implementations with the <see cref="INotificationSendersCollection"/>. The 
    /// availability and definition of these notifications is then passed on to all endpoints that are connected
    /// to the current endpoint. Upon reception of notification information an endpoint will generate a proxy for
    /// the notification interface thereby allowing remote listening to notification events through the proxy notification interface.
    /// </remarks>
    internal sealed class RemoteNotificationHub : RemoteEndpointProxyHub<NotificationSetProxy>, INotifyOfRemoteEndpointEvents
    {
        /// <summary>
        /// The collection that holds all the <see cref="INotificationSet"/> proxies for each endpoint that
        /// has been registered.
        /// </summary>
        private readonly IDictionary<EndpointId, IDictionary<Type, NotificationSetProxy>> m_RemoteNotifications
            = new Dictionary<EndpointId, IDictionary<Type, NotificationSetProxy>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteNotificationHub"/> class.
        /// </summary>
        /// <param name="endpointStateChange">The object that provides notification of the signing in and signing out of endpoints.</param>
        /// <param name="builder">The object that is responsible for building the command proxies.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostic methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointStateChange"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        internal RemoteNotificationHub(
            INotifyOfEndpointStateChange endpointStateChange,
            NotificationProxyBuilder builder,
            SystemDiagnostics systemDiagnostics)
            : base(
                endpointStateChange,
                (endpoint, type) => (NotificationSetProxy)builder.ProxyConnectingTo(endpoint, type),
                systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => builder);
            }
        }

        /// <summary>
        /// Extracts the correct collection of proxy types from the description.
        /// </summary>
        /// <param name="description">The object that contains the proxy type definitions.</param>
        /// <returns>The collection of proxy types.</returns>
        protected override IEnumerable<ISerializedType> ProxyTypesFromDescription(CommunicationDescription description)
        {
            return description.NotificationProxies;
        }

        /// <summary>
        /// Returns the name of the proxy objects for use in the trace logs.
        /// </summary>
        /// <returns>A string containing the name of the proxy objects for use in the trace logs.</returns>
        protected override string TraceNameForProxyObjects()
        {
            return "notifications";
        }

        /// <summary>
        /// Returns a value indicating if one or more proxies exist for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if one or more proxies exist for the endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected override bool HasProxyFor(EndpointId endpoint)
        {
            return m_RemoteNotifications.ContainsKey(endpoint);
        }

        /// <summary>
        /// Adds the collection of proxies to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="list">The collection of proxies.</param>
        protected override void AddProxiesToStorage(EndpointId endpoint, SortedList<Type, NotificationSetProxy> list)
        {
            if (!m_RemoteNotifications.ContainsKey(endpoint))
            {
                m_RemoteNotifications.Add(endpoint, list);
            }
            else
            {
                foreach (var pair in list)
                {
                    var existingList = (SortedList<Type, NotificationSetProxy>)m_RemoteNotifications[endpoint];
                    if (!existingList.ContainsKey(pair.Key))
                    {
                        existingList.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the proxy to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <param name="proxy">The proxy.</param>
        protected override void AddProxyFor(EndpointId endpoint, Type proxyType, NotificationSetProxy proxy)
        {
            if (m_RemoteNotifications.ContainsKey(endpoint))
            {
                var list = m_RemoteNotifications[endpoint];
                if (!list.ContainsKey(proxyType))
                {
                    list.Add(proxyType, proxy);
                }
            }
            else
            {
                var list = new SortedList<Type, NotificationSetProxy>(new TypeComparer())
                    {
                        {
                            proxyType, proxy
                        }
                    };
                m_RemoteNotifications.Add(endpoint, list);
            }
        }

        /// <summary>
        /// Removes all the proxies for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which all the proxies have to be removed.</param>
        protected override void RemoveProxiesFor(EndpointId endpoint)
        {
            if (m_RemoteNotifications.ContainsKey(endpoint))
            {
                var list = m_RemoteNotifications[endpoint];
                foreach (var pair in list)
                {
                    pair.Value.ClearAllEvents();
                }
            }

            m_RemoteNotifications.Remove(endpoint);
        }

        /// <summary>
        /// Returns a collection describing all the known endpoints and the notifications they
        /// provide.
        /// </summary>
        /// <returns>
        /// The collection describing all the known endpoints and the notifications they describe.
        /// </returns>
        public IEnumerable<NotificationInformationPerEndpoint> AvailableNotifications()
        {
            var result = new List<NotificationInformationPerEndpoint>();
            lock (m_Lock)
            {
                foreach (var pair in m_RemoteNotifications)
                {
                    var list = new List<Type>();
                    list.AddRange(pair.Value.Keys);
                    result.Add(new NotificationInformationPerEndpoint(pair.Key, list));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a collection describing all the known notifications for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     The collection describing all the known notifications for the given endpoint.
        /// </returns>
        public IEnumerable<Type> AvailableNotificationsFor(EndpointId endpoint)
        {
            var result = new List<Type>();
            lock (m_Lock)
            {
                if (m_RemoteNotifications.ContainsKey(endpoint))
                {
                    var list = m_RemoteNotifications[endpoint];
                    result.AddRange(list.Keys);
                }
            }

            return result;
        }

        /// <summary>
        /// An event raised when an endpoint signs on and provides a set of notifications.
        /// </summary>
        public event EventHandler<NotificationSetAvailabilityEventArgs> OnEndpointSignedIn;

        protected override void RaiseOnEndpointSignedIn(EndpointId endpoint, IEnumerable<Type> proxies)
        {
            var local = OnEndpointSignedIn;
            if (local != null)
            {
                local(this, new NotificationSetAvailabilityEventArgs(endpoint, proxies));
            }
        }

        /// <summary>
        /// An event raised when an endpoint signs off.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        protected override void RaiseOnEndpointSignedOff(EndpointId endpoint)
        {
            var local = OnEndpointSignedOff;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// Returns a value indicating if there are any known notifications for a given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if there are known notifications for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasNotificationsFor(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                return m_RemoteNotifications.ContainsKey(endpoint);
            }
        }

        /// <summary>
        /// Returns a value indicating if a specific set of notifications is available for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="notificationInterfaceType">The type of the notification that should be available.</param>
        /// <returns>
        ///     <see langword="true" /> if there are the specific notifications exist for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasNotificationFor(EndpointId endpoint, Type notificationInterfaceType)
        {
            lock (m_Lock)
            {
                if (m_RemoteNotifications.ContainsKey(endpoint))
                {
                    var commands = m_RemoteNotifications[endpoint];
                    return commands.ContainsKey(notificationInterfaceType);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the notification proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TNotification">The typeof notification set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the notifications should be returned.</param>
        /// <returns>The requested notification set.</returns>
        public TNotification NotificationsFor<TNotification>(EndpointId endpoint) where TNotification : class, INotificationSet
        {
            return NotificationsFor(endpoint, typeof(TNotification)) as TNotification;
        }

        /// <summary>
        /// Returns the notification proxy for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which the notification should be returned.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <returns>The requested notification set.</returns>
        public INotificationSet NotificationsFor(EndpointId endpoint, Type notificationType)
        {
            lock (m_Lock)
            {
                if (!m_RemoteNotifications.ContainsKey(endpoint))
                {
                    return null;
                }

                var notificationSets = m_RemoteNotifications[endpoint];
                if (!notificationSets.ContainsKey(notificationType))
                {
                    throw new NotificationNotSupportedException(notificationType);
                }

                var result = notificationSets[notificationType];
                return result;
            }
        }
    }
}
