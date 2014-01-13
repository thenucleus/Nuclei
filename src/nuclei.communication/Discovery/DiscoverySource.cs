//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
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
        /// The collection containing all the supported protocol versions.
        /// </summary>
        private readonly SortedSet<Version> m_SupportedProtocolVersions;

        /// <summary>
        /// The channel type used for discovery purposes.
        /// </summary>
        private readonly IDiscoveryChannelType m_Type;

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
        /// <param name="supportedProtocolVersions">An array containing all the supported versions of the communication protocol.</param>
        /// <param name="type">The channel type that is used to create WCF channels.</param>
        /// <param name="diagnostics">The object that provides the discovery information for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="supportedProtocolVersions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected DiscoverySource(Version[] supportedProtocolVersions, IDiscoveryChannelType type, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => supportedProtocolVersions);
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_SupportedProtocolVersions = new SortedSet<Version>(supportedProtocolVersions);
            m_Type = type;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<EndpointDiscoveredEventArgs> OnEndpointBecomingAvailable;

        protected void RaiseOnEndpointBecomingAvailable(IDiscoveryInformation info)
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
        public event EventHandler<EndpointLostEventArgs> OnEndpointBecomingUnavailable;

        protected void RaiseOnEndpointBecomingUnavailable(EndpointId id)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointLostEventArgs(id));
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
            get
            {
                return m_IsDiscoveryAllowed;
            }
        }

        protected void LocatedRemoteEndpointOnChannel(EndpointId id, Uri address)
        {
            var factory = CreateFactory(address);
            var service = factory.CreateChannel();
            var channel = (IChannel)service;
            try
            {
                IDiscoveryInformation discoveryInformation = null;

                var version = service.DiscoveryVersion();
                if (version >= DiscoveryVersions.Base)
                {
                    var protocolVersions = service.ProtocolVersions();
                    var intersection = protocolVersions
                        .Intersect(m_SupportedProtocolVersions)
                        .OrderBy(v => v);
                    if (!intersection.Any())
                    {
                        // There are no matching versions, don't bother with this endpoint.
                        return;
                    }

                    var highestVersion = intersection.First();
                    discoveryInformation = service.ConnectionInformationForProtocol(highestVersion);
                    if (discoveryInformation.ProtocolVersion == new Version())
                    {
                        // Something went wrong. Just ignore it then
                        return;
                    }
                }

                RaiseOnEndpointBecomingAvailable(discoveryInformation);
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
                channel.Close();
            }
        }

        private ChannelFactory<IDiscoveryInformationRespondingEndpointProxy> CreateFactory(Uri address)
        {
            var endpoint = new EndpointAddress(address);
            var binding = m_Type.GenerateBinding();

            return new ChannelFactory<IDiscoveryInformationRespondingEndpointProxy>(binding, endpoint);
        }

        protected void LostRemoteEndpointWithId(EndpointId id)
        {
            // Verify that channel is gone?

            RaiseOnEndpointBecomingUnavailable(id);
        }
    }
}