//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the TCP/IP discovery bootstrap channel.
    /// </summary>
    internal sealed class BootstrapChannel : IBootstrapChannel, IDisposable
    {
        // Has UDP discovery broadcast, provide only endpoint ID

        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// The type of channel that the bootstrap channel will use.
        /// </summary>
        private readonly IDiscoveryChannelType m_Type;

        /// <summary>
        /// the function used to build discovery endpoints.
        /// </summary>
        private readonly Func<IDiscoveryEndpoint> m_EndpointBuilder;

        /// <summary>
        /// The host information for the discovery host.
        /// </summary>
        private readonly IHoldServiceConnections m_Host;

        /// <summary>
        /// The WCF endpoint object for the current channel.
        /// </summary>
        private IDiscoveryEndpoint m_Endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapChannel"/> class.
        /// </summary>
        /// <param name="id">The ID of the endpoint that owns the current bootstrap channel.</param>
        /// <param name="type">The channel type that should be used for the current bootstrap channel.</param>
        /// <param name="endpointBuilder">The function that builds WCF endpoints.</param>
        /// <param name="host">
        /// The object that handles the <see cref="ServiceHost"/> for the channel used to communicate with.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="host"/> is <see langword="null" />.
        /// </exception>
        public BootstrapChannel(
            EndpointId id, 
            IDiscoveryChannelType type, 
            Func<IDiscoveryEndpoint> endpointBuilder, 
            IHoldServiceConnections host)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => endpointBuilder);
                Lokad.Enforce.Argument(() => host);
            }

            m_Id = id;
            m_Type = type;
            m_EndpointBuilder = endpointBuilder;
            m_Host = host;
        }

        /// <summary>
        /// Opens the channel.
        /// </summary>
        public void OpenChannel()
        {
            m_Endpoint = m_EndpointBuilder();
            Func<ServiceHost, ServiceEndpoint> endpointBuilder =
                host =>
                {
                    var dataEndpoint = m_Type.AttachDiscoveryEndpoint(host, typeof(IDiscoveryEndpoint), m_Id);
                    return dataEndpoint;
                };
            m_Host.OpenChannel(m_Endpoint, endpointBuilder);
        }

        /// <summary>
        /// Closes the channel.
        /// </summary>
        public void CloseChannel()
        {
            m_Host.CloseConnection();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CloseChannel();
        }
    }
}
