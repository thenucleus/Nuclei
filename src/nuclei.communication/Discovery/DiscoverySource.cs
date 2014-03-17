//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Nuclei.Communication.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// The base class for classes that handle discovery of remote endpoints.
    /// </summary>
    internal abstract class DiscoverySource : IDiscoverOtherServices
    {
        /// <summary>
        /// The collection containing the translators mapped to the version of the discovery channel
        /// they work with.
        /// </summary>
        private readonly SortedList<Version, ITranslateVersionedChannelInformation> m_TranslatorMap
            = new SortedList<Version, ITranslateVersionedChannelInformation>();

        /// <summary>
        /// The function that is used to get the channel template used for discovery purposes.
        /// </summary>
        private readonly Func<ChannelTemplate, IDiscoveryChannelTemplate> m_TemplateBuilder;

        /// <summary>
        /// The object that provides the diagnostics for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Indicates if discovery information should be passed on or not.
        /// </summary>
        private volatile bool m_IsDiscoveryAllowed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoverySource"/> class.
        /// </summary>
        /// <param name="translatorsByVersion">
        ///     An array containing all the supported translators mapped to the version of the discovery layer.
        /// </param>
        /// <param name="templateBuilder">The function that is used to create the channel template that is used to create WCF channels.</param>
        /// <param name="diagnostics">The object that provides the discovery information for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="translatorsByVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="templateBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected DiscoverySource(
            Tuple<Version, ITranslateVersionedChannelInformation>[] translatorsByVersion,
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder, 
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => translatorsByVersion);
                Lokad.Enforce.Argument(() => templateBuilder);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            foreach (var pair in translatorsByVersion)
            {
                m_TranslatorMap.Add(pair.Item1, pair.Item2);
            }

            m_TemplateBuilder = templateBuilder;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<EndpointDiscoveredEventArgs> OnEndpointBecomingAvailable;

        private void RaiseOnEndpointBecomingAvailable(EndpointInformation info)
        {
            var local = OnEndpointBecomingAvailable;
            if (local != null)
            {
                local(this, new EndpointDiscoveredEventArgs(info));
            }
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointBecomingUnavailable;

        private void RaiseOnEndpointBecomingUnavailable(EndpointId id)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
            }
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public virtual void StartDiscovery()
        {
            m_IsDiscoveryAllowed = true;
        }

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        public virtual void EndDiscovery()
        {
            m_IsDiscoveryAllowed = false;
        }

        /// <summary>
        /// Gets a value indicating whether discovery is allowed or not.
        /// </summary>
        protected bool IsDiscoveryAllowed
        {
            [DebuggerStepThrough]
            get
            {
                return m_IsDiscoveryAllowed;
            }
        }

        protected void LocatedRemoteEndpointOnChannel(EndpointId id, Uri address)
        {
            var pair = GetMostSuitableDiscoveryChannel(address);
            if (pair == null)
            {
                m_Diagnostics.Log(
                    LevelToLog.Info,
                    CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_DiscoveryFailedToFindMatchingDiscoveryVersion_WithUri,
                            address));

                return;
            }

            Debug.Assert(m_TranslatorMap.ContainsKey(pair.Item1), "There should be a translator for the given version.");
            var translator = m_TranslatorMap[pair.Item1];
            var protocol = translator.FromUri(pair.Item2);
            if (protocol == null)
            {
                return;
            }

            var discoveryInfo = new DiscoveryInformation(address);
            var info = new EndpointInformation(id, discoveryInfo, protocol);
            RaiseOnEndpointBecomingAvailable(info);
        }

        private Tuple<Version, Uri> GetMostSuitableDiscoveryChannel(Uri address)
        {
            var factory = CreateFactoryForBootstrapChannel(address);
            var service = factory.CreateChannel();
            var channel = (IChannel)service;
            try
            {
                var versions = service.DiscoveryVersions();

                var intersection = versions
                    .Intersect(m_TranslatorMap.Keys)
                    .OrderByDescending(v => v);
                if (!intersection.Any())
                {
                    return null;
                }

                var highestVersion = intersection.First();
                var uri = service.UriForVersion(highestVersion);
                return uri != null ? new Tuple<Version, Uri>(highestVersion, uri) : null;
            }
            catch (FaultException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Warn,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_DiscoveryFailedToConnectToEndpoint_WithUriAndError,
                        address,
                        e));
            }
            catch (CommunicationException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Warn,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_DiscoveryFailedToConnectToEndpoint_WithUriAndError,
                        address,
                        e));
            }
            finally
            {
                try
                {
                    channel.Close();
                }
                catch (CommunicationObjectFaultedException e)
                {
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            factory.Endpoint.Address.Uri,
                            e));
                }
                catch (ProtocolException e)
                {
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            factory.Endpoint.Address.Uri,
                            e));
                }
                catch (CommunicationObjectAbortedException e)
                {
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            factory.Endpoint.Address.Uri,
                            e));
                }
                catch (TimeoutException e)
                {
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            factory.Endpoint.Address.Uri,
                            e));
                }
            }

            return null;
        }

        private ChannelFactory<IBootstrapEndpointProxy> CreateFactoryForBootstrapChannel(Uri address)
        {
            var template = m_TemplateBuilder(address.ToChannelTemplate());

            var endpoint = new EndpointAddress(address);
            var binding = template.GenerateBinding();

            return new ChannelFactory<IBootstrapEndpointProxy>(binding, endpoint);
        }

        protected void LostRemoteEndpointWithId(EndpointId id)
        {
            // Don't need to verify that the channel is actually gone because the only
            // way we should be geting this message is from the channel itself.
            RaiseOnEndpointBecomingUnavailable(id);
        }
    }
}
