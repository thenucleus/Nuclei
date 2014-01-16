//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Nuclei.Communication.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines the versioned translator which reads channel information for a version 1.0 discovery channel.
    /// </summary>
    internal sealed class DiscoveryChannelTranslator : ITranslateVersionedChannelInformation
    {
        /// <summary>
        /// The collection containing the translators mapped to the version of the discovery channel
        /// they work with.
        /// </summary>
        private readonly SortedSet<Version> m_ProtocolVersions;

        /// <summary>
        /// The channel type used for discovery purposes.
        /// </summary>
        private readonly IDiscoveryChannelType m_Type;

        /// <summary>
        /// The object that provides the diagnostics for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryChannelTranslator"/> class.
        /// </summary>
        /// <param name="protocolVersions">
        ///     The collection containing all the versions of the protocol layer.
        /// </param>
        /// <param name="type">The channel type used for discovery purposes.</param>
        /// <param name="diagnostics">The object that provides the diagnostics for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolVersions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public DiscoveryChannelTranslator(
            Version[] protocolVersions,
            IDiscoveryChannelType type,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => protocolVersions);
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_ProtocolVersions = new SortedSet<Version>(protocolVersions);
            m_Type = type;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Returns channel information obtained from a specific versioned discovery channel.
        /// </summary>
        /// <param name="address">The address of the versioned discovery channel.</param>
        /// <returns>The channel information.</returns>
        public ChannelInformation FromUri(Uri address)
        {
            var factory = CreateFactoryForDiscoveryChannel(address);
            var service = factory.CreateChannel();
            var channel = (IChannel)service;
            try
            {
                var serviceVersion = service.Version();
                if (serviceVersion != DiscoveryVersions.V1)
                {
                    throw new IncorrectTranslatorVersionException();
                }

                var versions = service.ProtocolVersions();
                var intersection = versions
                    .Intersect(m_ProtocolVersions)
                    .OrderBy(v => v);
                if (!intersection.Any())
                {
                    return null;
                }

                var highestVersion = intersection.First();
                var localInfo = service.ConnectionInformationForProtocol(highestVersion);
                return ChannelInformationToTransportConverter.FromVersioned(localInfo);
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

        private ChannelFactory<IInformationEndpointProxy> CreateFactoryForDiscoveryChannel(Uri address)
        {
            var endpoint = new EndpointAddress(address);
            var binding = m_Type.GenerateBinding();

            return new ChannelFactory<IInformationEndpointProxy>(binding, endpoint);
        }
    }
}
