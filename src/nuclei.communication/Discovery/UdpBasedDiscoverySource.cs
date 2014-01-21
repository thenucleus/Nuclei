//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Handles the discovery of endpoints on other applications to which a connection can be 
    /// made via one of the WCF protocols.
    /// </summary>
    internal sealed class UdpBasedDiscoverySource : DiscoverySource, IDisposable
    {
        private static EndpointId GetEndpointId(EndpointDiscoveryMetadata metadata)
        {
            XElement endpointElement = metadata.Extensions.Elements("EndpointId").FirstOrDefault();
            if (endpointElement == null)
            {
                throw new MissingEndpointIdException();
            }

            return new EndpointId(endpointElement.Value);
        }

        /// <summary>
        /// The service that handles the detection of discovery announcements.
        /// </summary>
        private ServiceHost m_Host;

        /// <summary>
        /// The object used to locate the services that are already online when the
        /// current application comes online.
        /// </summary>
        private DiscoveryClient m_DiscoveryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpBasedDiscoverySource"/> class.
        /// </summary>
        /// <param name="translatorsByVersion">
        ///     An array containing all the supported translators mapped to the version of the discovery layer.
        /// </param>
        /// <param name="template">The channel type that is used to create WCF channels.</param>
        /// <param name="diagnostics">The object that provides the discovery information for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="translatorsByVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="template"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public UdpBasedDiscoverySource(
            Tuple<Version, ITranslateVersionedChannelInformation>[] translatorsByVersion,
            IDiscoveryChannelTemplate template,
            SystemDiagnostics diagnostics)
            : base(translatorsByVersion, template, diagnostics)
        {
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public override void StartDiscovery()
        {
            base.StartDiscovery();

            var service = new AnnouncementService();
            service.OnlineAnnouncementReceived += (s, e) => HandleOnlineAnnouncementReceived(e.EndpointDiscoveryMetadata);
            service.OfflineAnnouncementReceived += (se, e) => HandleOfflineAnnouncementReceived(e.EndpointDiscoveryMetadata);

            m_Host = new ServiceHost(service);
            m_Host.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            m_Host.Open();

            // When we log on we also need to get the information from the services
            // that are currently already online
            LocateExistingEndpoints();
        }

        private void HandleOnlineAnnouncementReceived(EndpointDiscoveryMetadata metadata)
        {
            // Only acknowledge service contracts that we actually know about.
            if (metadata.ContractTypeNames.FirstOrDefault(x => x.Name == typeof(IMessageReceivingEndpoint).Name) != null)
            {
                var id = GetEndpointId(metadata);
                var address = metadata.Address.Uri;

                // Note that the EndpointId meta data is defined by the BootstrapChannel
                // Do we want to thread this?
                LocatedRemoteEndpointOnChannel(id, address);
            }
        }

        private void HandleOfflineAnnouncementReceived(EndpointDiscoveryMetadata metadata)
        {
            // Only acknowledge service contracts that we actually know about.
            if (metadata.ContractTypeNames.FirstOrDefault(x => x.Name == typeof(IMessageReceivingEndpoint).Name) != null)
            {
                var id = GetEndpointId(metadata);
                LostRemoteEndpointWithId(id);
            }
        }

        private void LocateExistingEndpoints()
        {
            m_DiscoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            m_DiscoveryClient.FindProgressChanged += (s, e) => HandleOnlineAnnouncementReceived(e.EndpointDiscoveryMetadata);
            m_DiscoveryClient.FindCompleted += OnFindCompleted;

            m_DiscoveryClient.FindAsync(new FindCriteria(typeof(IMessageReceivingEndpoint)));
        }

        private void OnFindCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    return;
                }

                if (e.Error != null)
                {
                    throw e.Error;
                }
            }
            finally
            {
                CloseDiscoveryClient();
            }
        }

        private void CloseDiscoveryClient()
        {
            if (m_DiscoveryClient != null)
            {
                if (m_DiscoveryClient.InnerChannel.State == CommunicationState.Opened)
                {
                    m_DiscoveryClient.Close();
                }

                var disposable = m_DiscoveryClient as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                m_DiscoveryClient = null;
            }
        }

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        public override void EndDiscovery()
        {
            CloseDiscoveryClient();
            CleanupHost();

            base.EndDiscovery();
        }

        private void CleanupHost()
        {
            if (m_Host != null)
            {
                m_Host.Close();

                var disposable = m_Host as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                m_Host = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CloseDiscoveryClient();
            CleanupHost();
        }
    }
}
