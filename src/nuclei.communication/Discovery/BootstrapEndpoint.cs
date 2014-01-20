//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines an endpoint that will provide information about all known versioned discovery
    /// endpoints for an application.
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class BootstrapEndpoint : IBootstrapEndpoint
    {
        /// <summary>
        /// The collection that maps a discovery version to a channel URI.
        /// </summary>
        private readonly Dictionary<Version, Uri> m_VersionToUriMap
            = new Dictionary<Version, Uri>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapEndpoint"/> class.
        /// </summary>
        /// <param name="discoveryChannelsByVersion">The collection that maps a discovery version to a discovery channel URI.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoveryChannelsByVersion"/> is <see langword="null" />.
        /// </exception>
        public BootstrapEndpoint(IEnumerable<Tuple<Version, Uri>> discoveryChannelsByVersion)
        {
            {
                Lokad.Enforce.Argument(() => discoveryChannelsByVersion);
            }

            foreach (var pair in discoveryChannelsByVersion)
            {
                m_VersionToUriMap.Add(pair.Item1, pair.Item2);
            }
        }

        /// <summary>
        /// Returns the supported versions of the discovery layer.
        /// </summary>
        /// <returns>A collection containing all the supported versions of the discovery layer.</returns>
        public Version[] DiscoveryVersions()
        {
            return m_VersionToUriMap.Keys
                .OrderBy(v => v)
                .ToArray();
        }

        /// <summary>
        /// Returns the URI of the discovery channel with the given version.
        /// </summary>
        /// <param name="version">The version of the discovery channel.</param>
        /// <returns>The URI of the discovery channel with the given version.</returns>
        public Uri UriForVersion(Version version)
        {
            if (version == null)
            {
                return null;
            }

            if (!m_VersionToUriMap.ContainsKey(version))
            {
                return null;
            }

            return m_VersionToUriMap[version];
        }
    }
}
