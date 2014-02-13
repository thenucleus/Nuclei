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
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// The base class for classes that store proxies for one or more remote endpoints.
    /// </summary>
    /// <typeparam name="TProxyObject">The base type of the proxy object that is created by the proxy builder.</typeparam>
    internal abstract class RemoteEndpointProxyHub<TProxyObject> : INotifyOfEndpointStateChange where TProxyObject : class
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The object that stores information about the known endpoints.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_EndpointInformationStorage;

        /// <summary>
        /// The function that creates proxy objects.
        /// </summary>
        private readonly Func<EndpointId, Type, TProxyObject> m_Builder;

        /// <summary>
        /// The object that provides the diagnostic methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEndpointProxyHub{TProxyObject}"/> class.
        /// </summary>
        /// <param name="endpointInformationStorage">The object that provides notification of the signing in and signing out of endpoints.</param>
        /// <param name="builder">The function that is responsible for building the proxies.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointInformationStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        protected RemoteEndpointProxyHub(
            IStoreInformationAboutEndpoints endpointInformationStorage,
            Func<EndpointId, Type, TProxyObject> builder,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => endpointInformationStorage);
                Lokad.Enforce.Argument(() => builder);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_EndpointInformationStorage = endpointInformationStorage;
            m_EndpointInformationStorage.OnEndpointDisconnected += HandleEndpointSignOut;

            m_Builder = builder;
            m_Diagnostics = systemDiagnostics;
        }

        protected void OnReceiptOfEndpointProxies(EndpointId endpoint, IEnumerable<OfflineTypeInformation> proxyTypes)
        {
            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Received endpoint information"))
            {
                var proxyList = new List<Type>();
                try
                {
                    lock (m_Lock)
                    {
                        Debug.Assert(!HasProxyFor(endpoint), "There shouldn't be any endpoint information");

                        // We expect that each endpoint will only have a few proxy types but there might be a lot of 
                        // endpoints. Also accessing any method in a proxy is going to be slow because it 
                        // needs to travel to another application and come back (possibly over the network). it seems the
                        // sorted list is the best trade-off (memory vs performance) in this case.
                        m_Diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Received {0} {1} from endpoint [{2}].",
                                proxyTypes.Count(),
                                TraceNameForProxyObjects(),
                                endpoint));

                        var list = new SortedList<Type, TProxyObject>(proxyTypes.Count(), new TypeComparer());
                        foreach (var proxy in proxyTypes)
                        {
                            // Hydrate the proxy type. This requires loading the assembly which a) might be slow and b) might fail
                            try
                            {
                                var proxyType = LoadProxyType(endpoint, proxy);
                                list.Add(proxyType, m_Builder(endpoint, proxyType));
                            }
                            catch (UnableToGenerateProxyException)
                            {
                                // The generation of the proxy failed somehow. Let's just ignore it for now.
                            }
                        }

                        if (list.Count > 0)
                        {
                            AddProxiesToStorage(endpoint, list);
                            proxyList.AddRange(list.Keys);
                        }
                    }
                }
                catch (AggregateException)
                {
                    // We don't really care about any exceptions that were thrown. If there was a problem we just remove the endpoint from the
                    // 'waiting list' and move on.
                }

                // Notify the outside world that we have more proxies. Do this outside the lock because a) the notification may take a while and 
                // b) it may trigger all kinds of other mayhem.
                using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Notifying of new endpoint"))
                {
                    RaiseOnEndpointConnected(endpoint);
                }
            }
        }

        private Type LoadProxyType(EndpointId endpoint, OfflineTypeInformation serializedType)
        {
            // Hydrate the proxy type. This requires loading the assembly which a) might
            // be slow and b) might fail
            Type proxyType;
            try
            {
                proxyType = serializedType.ToType();

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Got {0} from endpoint [{1}] of type {2}.",
                        TraceNameForProxyObjects(),
                        endpoint,
                        proxyType));
            }
            catch (UnableToLoadOfflineTypeException)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not load the {0} type: {1} for endpoint {2}",
                        TraceNameForProxyObjects(),
                        serializedType.TypeFullName,
                        endpoint));

                throw;
            }

            return proxyType;
        }

        private void HandleEndpointSignOut(object sender, EndpointEventArgs eventArgs)
        {
            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Endpoint disconnected - removing proxies."))
            {
                lock (m_Lock)
                {
                    if (HasProxyFor(eventArgs.Endpoint))
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Removing {0} for endpoint [{1}].",
                                TraceNameForProxyObjects(),
                                eventArgs.Endpoint));

                        RemoveProxiesFor(eventArgs.Endpoint);
                    }
                }

                using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Notifying of proxy removal."))
                {
                    RaiseOnEndpointDisconnected(eventArgs.Endpoint);
                }
            }
        }

        /// <summary>
        /// The object used to lock on. Provided to allow derivative classes to use the same lock as the base class.
        /// </summary>
        protected object Lock
        {
            get
            {
                return m_Lock;
            }
        }

        /// <summary>
        /// Returns the name of the proxy objects for use in the trace logs.
        /// </summary>
        /// <returns>A string containing the name of the proxy objects for use in the trace logs.</returns>
        protected abstract string TraceNameForProxyObjects();

        /// <summary>
        /// Returns a value indicating if one or more proxies exist for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if one or more proxies exist for the endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected abstract bool HasProxyFor(EndpointId endpoint);

        /// <summary>
        /// Adds the collection of proxies to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="list">The collection of proxies.</param>
        protected abstract void AddProxiesToStorage(EndpointId endpoint, SortedList<Type, TProxyObject> list);

        /// <summary>
        /// Adds the proxy to the storage.
        /// </summary>
        /// <param name="endpoint">The endpoint from which the proxies came.</param>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <param name="proxy">The proxy.</param>
        protected abstract void AddProxyFor(EndpointId endpoint, Type proxyType, TProxyObject proxy);

        /// <summary>
        /// Removes all the proxies for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which all the proxies have to be removed.</param>
        protected abstract void RemoveProxiesFor(EndpointId endpoint);

        /// <summary>
        /// An event raised when an endpoint has signed in.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointConnected;

        private void RaiseOnEndpointConnected(EndpointId endpoint)
        {
            var local = OnEndpointConnected;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// An event raised when an endpoint has signed out.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointDisconnected;

        /// <summary>
        /// Indicates that an endpoint has signed off.
        /// </summary>
        /// <param name="endpoint">The endpoint that signed off.</param>
        private void RaiseOnEndpointDisconnected(EndpointId endpoint)
        {
            var local = OnEndpointDisconnected;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }
    }
}
