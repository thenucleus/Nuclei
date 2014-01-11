using System;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// The base class for classes that handle discovery of remote endpoints.
    /// </summary>
    internal abstract class DiscoverySource : IDiscoverOtherServices
    {
        /// <summary>
        /// Indicates if discovery information should be passed on or not.
        /// </summary>
        private volatile bool m_IsDiscoveryAllowed;

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
            // Query endpoint for information and then push it out.


            RaiseOnEndpointBecomingAvailable(new ChannelConnectionInformation(id, bindingType, address));
        }

        protected void LostRemoteEndpointWithId(EndpointId id)
        {
            // Verify that channel is gone?

            RaiseOnEndpointBecomingUnavailable(id);
        }
    }
}