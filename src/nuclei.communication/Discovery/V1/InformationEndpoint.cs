using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines the WCF endpoint that responds to discovery requests.
    /// </summary>
    internal sealed class InformationEndpoint : IInformationEndpoint
    {
        /// <summary>
        /// Defines an discovery information object that carries information about a null, or non-existant, protocol.
        /// </summary>
        private sealed class NullDiscoveryInformation : IDiscoveryInformation
        {
            /// <summary>
            /// Gets the version of the information object.
            /// </summary>
            public Version ProtocolVersion
            {
                get
                {
                    return new Version();
                }
            }
        }

        /// <summary>
        /// The collection containing the information about all the versions of the protocol layer.
        /// </summary>
        private readonly SortedList<Version, IDiscoveryInformation> m_ProtocolInformation
            = new SortedList<Version, IDiscoveryInformation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationEndpoint"/> class.
        /// </summary>
        /// <param name="protocolInformation">The array containing the information about all the versions of the protocol layer.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolInformation"/> is <see langword="null" />.
        /// </exception>
        public InformationEndpoint(IDiscoveryInformation[] protocolInformation)
        {
            {
                Lokad.Enforce.Argument(() => protocolInformation);
            }

            foreach (var entry in protocolInformation)
            {
                m_ProtocolInformation.Add(entry.ProtocolVersion, entry);
            }
        }

        /// <summary>
        /// Returns the version of the discovery protocol.
        /// </summary>
        /// <returns>The version of the discovery protocol.</returns>
        public Version Version()
        {
            return DiscoveryVersions.V1;
        }

        /// <summary>
        /// Returns an array containing all the versions of the supported communication protocols.
        /// </summary>
        /// <returns>An array containing the versions of the supported communication protocols.</returns>
        public Version[] ProtocolVersions()
        {
            return m_ProtocolInformation.Keys
                .OrderBy(v => v)
                .ToArray();
        }

        /// <summary>
        /// Returns the discovery information for the communication protocol with the given version.
        /// </summary>
        /// <param name="version">The version of the protocol for which the discovery information should be provided.</param>
        /// <returns>The discovery information for the communication protocol with the given version.</returns>
        public IDiscoveryInformation ConnectionInformationForProtocol(Version version)
        {
            if (version == null)
            {
                return new NullDiscoveryInformation();
            }

            if (!m_ProtocolInformation.ContainsKey(version))
            {
                return new NullDiscoveryInformation();
            }

            return m_ProtocolInformation[version];
        }
    }
}
