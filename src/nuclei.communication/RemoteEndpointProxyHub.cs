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

namespace Nuclei.Communication
{
    /// <summary>
    /// The base class for classes that store proxies for one or more remote endpoints.
    /// </summary>
    /// <typeparam name="TProxyObject">The base type of the proxy object that is created by the proxy builder.</typeparam>
    internal abstract class RemoteEndpointProxyHub<TProxyObject> where TProxyObject : class
    {
        /// <summary>
        /// The collection of endpoints which have been contacted for information about 
        /// their available proxies.
        /// </summary>
        private readonly IList<EndpointId> m_WaitingForEndpointInformation
            = new List<EndpointId>();

        /// <summary>
        /// The function that creates proxy objects.
        /// </summary>
        private readonly Func<EndpointId, Type, TProxyObject> m_Builder;

        /// <summary>
        /// The object that provides the diagnostic methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        protected readonly object m_Lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEndpointProxyHub{TProxyObject}"/> class.
        /// </summary>
        /// <param name="endpointStateChange">The object that provides notification of the signing in and signing out of endpoints.</param>
        /// <param name="builder">The function that is responsible for building the proxies.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointStateChange"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        protected RemoteEndpointProxyHub(
            INotifyOfEndpointStateChange endpointStateChange,
            Func<EndpointId, Type, TProxyObject> builder,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => endpointStateChange);
                Lokad.Enforce.Argument(() => builder);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            endpointStateChange.OnEndpointConnected += HandleEndpointSignIn;
            endpointStateChange.OnEndpointDisconnected += HandleEndpointSignOut;

            m_Builder = builder;
            m_Diagnostics = systemDiagnostics;
        }

        private void HandleEndpointSignIn(object sender, EndpointSignInEventArgs eventArgs)
        {
            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Received endpoint information"))
            {
                var endpoint = eventArgs.ConnectionInformation.Id;

                bool haveStoredEndpointInformation = false;
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
                        var proxyTypes = ProxyTypesFromDescription(eventArgs.Description);
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
                            catch (UnableToLoadProxyTypeException)
                            {
                                // Unable to load the proxy type. Let's just ignore it for now.
                            }
                            catch (UnableToGenerateProxyException)
                            {
                                // The generation of the proxy failed somehow. Let's just ignore it for now.
                            }
                        }

                        if (list.Count > 0)
                        {
                            AddProxiesToStorage(endpoint, list);
                            haveStoredEndpointInformation = true;
                            proxyList.AddRange(list.Keys);
                        }
                    }
                }
                catch (AggregateException)
                {
                    // We don't really care about any exceptions that were thrown. If there was a problem we just remove the endpoint from the
                    // 'waiting list' and move on.
                    haveStoredEndpointInformation = false;
                }
                finally
                {
                    lock (m_Lock)
                    {
                        if (m_WaitingForEndpointInformation.Contains(endpoint))
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Trace,
                                CommunicationConstants.DefaultLogTextPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "No longer waiting for {0} from endpoint: {1}",
                                    TraceNameForProxyObjects(),
                                    endpoint));

                            m_WaitingForEndpointInformation.Remove(endpoint);
                        }
                    }
                }

                // Notify the outside world that we have more proxies. Do this outside the lock because a) the notification may take a while and 
                // b) it may trigger all kinds of other mayhem.
                if (haveStoredEndpointInformation)
                {
                    if (proxyList.Count > 0)
                    {
                        using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Notifying of new endpoint"))
                        {
                            RaiseOnEndpointSignedIn(endpoint, proxyList);
                        }
                    }
                }
            }
        }

        private void HandleEndpointSignOut(object sender, EndpointSignedOutEventArgs eventArgs)
        {
            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Endpoint disconnected - removing proxies."))
            {
                lock (m_Lock)
                {
                    if (m_WaitingForEndpointInformation.Contains(eventArgs.Endpoint))
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "No longer waiting for {0} from endpoint [{1}].",
                                TraceNameForProxyObjects(),
                                eventArgs.Endpoint));

                        m_WaitingForEndpointInformation.Remove(eventArgs.Endpoint);
                    }

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
                    RaiseOnEndpointSignedOff(eventArgs.Endpoint);
                }
            }
        }

        private Type LoadProxyType(EndpointId endpoint, ISerializedType serializedType)
        {
            // Hydrate the proxy type. This requires loading the assembly which a) might
            // be slow and b) might fail
            Type proxyType;
            try
            {
                proxyType = ProxyExtensions.ToType(serializedType);

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
            catch (UnableToLoadProxyTypeException)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not load the {0} type: {1} for endpoint {2}",
                        TraceNameForProxyObjects(),
                        serializedType.AssemblyQualifiedTypeName,
                        endpoint));

                throw;
            }

            return proxyType;
        }

        /// <summary>
        /// Extracts the correct collection of proxy types from the description.
        /// </summary>
        /// <param name="description">The object that contains the proxy type definitions.</param>
        /// <returns>The collection of proxy types.</returns>
        protected abstract IEnumerable<ISerializedType> ProxyTypesFromDescription(CommunicationDescription description);

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
        /// Indicates that a new endpoint has signed in and all the proxy information has been obtained.
        /// </summary>
        /// <param name="endpoint">The endpoint that has signed in.</param>
        /// <param name="proxies">The proxy types for the given endpoint.</param>
        protected abstract void RaiseOnEndpointSignedIn(EndpointId endpoint, IEnumerable<Type> proxies);

        /// <summary>
        /// Indicates that an endpoint has signed off.
        /// </summary>
        /// <param name="endpoint">The endpoint that signed off.</param>
        protected abstract void RaiseOnEndpointSignedOff(EndpointId endpoint);
    }
}
