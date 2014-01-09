//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml.Linq;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Handles the discovery of endpoints on other applications to which a connection can be 
    /// made via one of the WCF protocols.
    /// </summary>
    internal sealed class UdpBasedDiscoverySource : IDiscoverOtherServices, IDisposable
    {
        // Note that the EndpointId meta data is defined by the TcpChannelType
        private static EndpointId GetEndpointId(EndpointDiscoveryMetadata metadata)
        {
            XElement endpointElement = metadata.Extensions.Elements("EndpointId").FirstOrDefault();
            if (endpointElement == null)
            {
                throw new MissingEndpointIdException();
            }

            return new EndpointId(endpointElement.Value);
        }

        // Note that the BindingType meta data is defined by the TcpChannelType
        private static ChannelType GetBindingType(EndpointDiscoveryMetadata metadata)
        {
            XElement bindingTypeElement = metadata.Extensions.Elements("BindingType").FirstOrDefault();
            if (bindingTypeElement == null)
            {
                throw new MissingBindingTypeException();
            }

            var type = (ChannelType)Enum.Parse(typeof(ChannelType), bindingTypeElement.Value);

            Debug.Assert(type != ChannelType.None, "Found an incorrect binding type.");
            return type;
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
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<EndpointDiscoveredEventArgs> OnEndpointBecomingAvailable;

        private void RaiseOnEndpointBecomingAvailable(ChannelConnectionInformation info)
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

        private void RaiseOnEndpointBecomingUnavailable(EndpointId id, ChannelType channelType)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointLostEventArgs(id, channelType));
            }
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public void StartDiscovery()
        {
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
                var bindingType = GetBindingType(metadata);
                var address = metadata.Address.Uri;

                RaiseOnEndpointBecomingAvailable(new ChannelConnectionInformation(id, bindingType, address));
            }
        }

        private void HandleOfflineAnnouncementReceived(EndpointDiscoveryMetadata metadata)
        {
            // Only acknowledge service contracts that we actually know about.
            if (metadata.ContractTypeNames.FirstOrDefault(x => x.Name == typeof(IMessageReceivingEndpoint).Name) != null)
            {
                var id = GetEndpointId(metadata);
                var bindingType = GetBindingType(metadata);
                RaiseOnEndpointBecomingUnavailable(id, bindingType);
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
        public void EndDiscovery()
        {
            CloseDiscoveryClient();
            CleanupHost();
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
