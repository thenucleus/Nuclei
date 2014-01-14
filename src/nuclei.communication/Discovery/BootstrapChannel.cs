using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the TCP/IP discovery bootstrap channel.
    /// </summary>
    internal sealed class BootstrapChannel : IBootstrapChannel, IDisposable
    {
        /// <summary>
        /// The collection that stores the hosts based on the version of the 
        /// discovery layer they provide.
        /// </summary>
        private readonly Dictionary<Version, IHoldServiceConnections> m_HostsByVersion
            = new Dictionary<Version, IHoldServiceConnections>();

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
        private readonly Func<Version, Tuple<Type, IVersionedDiscoveryEndpoint>> m_VersionedEndpointBuilder;

        /// <summary>
        /// The function that creates the host information for the discovery host.
        /// </summary>
        private readonly Func<IHoldServiceConnections> m_HostBuilder;

        /// <summary>
        /// The host for the discovery bootstrap channel.
        /// </summary>
        private IHoldServiceConnections m_BootstrapHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapChannel"/> class.
        /// </summary>
        /// <param name="id">The ID of the endpoint that owns the current bootstrap channel.</param>
        /// <param name="type">The channel type that should be used for the current bootstrap channel.</param>
        /// <param name="versionedEndpointBuilder">The function that builds WCF endpoints.</param>
        /// <param name="hostBuilder">
        /// The function that returns an object which handles the <see cref="ServiceHost"/> for the channel used to communicate with.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="versionedEndpointBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hostBuilder"/> is <see langword="null" />.
        /// </exception>
        public BootstrapChannel(
            EndpointId id, 
            IDiscoveryChannelType type,
            Func<Version, Tuple<Type, IVersionedDiscoveryEndpoint>> versionedEndpointBuilder, 
            Func<IHoldServiceConnections> hostBuilder)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => versionedEndpointBuilder);
                Lokad.Enforce.Argument(() => hostBuilder);
            }

            m_Id = id;
            m_Type = type;
            m_VersionedEndpointBuilder = versionedEndpointBuilder;
            m_HostBuilder = hostBuilder;
        }

        /// <summary>
        /// Opens the channel.
        /// </summary>
        public void OpenChannel()
        {
            var discoveryChannelsByVersion = new Dictionary<Version, Uri>();
            foreach (var version in DiscoveryVersions.SupportedVersions())
            {
                var host = m_HostBuilder();
                var endpoint = m_VersionedEndpointBuilder(version);

                var localVersion = version;
                var type = endpoint.Item1;
                Func<ServiceHost, ServiceEndpoint> endpointBuilder = h => m_Type.AttachVersionedDiscoveryEndpoint(h, type, localVersion);
                var uri = host.OpenChannel(endpoint.Item2, endpointBuilder);
                
                m_HostsByVersion.Add(version, host);
                discoveryChannelsByVersion.Add(version, uri);
            }

            // Open the channel that provides the discovery of the discovery channels
            m_BootstrapHost = m_HostBuilder();
            var bootstrapEndpoint = new BootstrapEndpoint(discoveryChannelsByVersion.Select(p => new Tuple<Version, Uri>(p.Key, p.Value)));

            Func<ServiceHost, ServiceEndpoint> bootstrapEndpointBuilder = 
                h => m_Type.AttachDiscoveryEntryEndpoint(h, typeof(IBootstrapEndpoint), m_Id);
            m_BootstrapHost.OpenChannel(bootstrapEndpoint, bootstrapEndpointBuilder);
        }

        /// <summary>
        /// Closes the channel.
        /// </summary>
        public void CloseChannel()
        {
            m_BootstrapHost.CloseConnection();
            foreach (var pair in m_HostsByVersion)
            {
                pair.Value.CloseConnection();
            }
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
