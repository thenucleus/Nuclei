//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Stores information about one or more protocol channels.
    /// </summary>
    /// <remarks>
    /// This class should never be changed so that it is always backwards compatible.
    /// </remarks>
    internal sealed class VersionedChannelInformation
    {
        /// <summary>
        /// The version of the protocol for the current channel.
        /// </summary>
        private readonly Version m_ProtocolVersion;

        /// <summary>
        /// The address of the given channel.
        /// </summary>
        private readonly Uri m_Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedChannelInformation"/> class.
        /// </summary>
        /// <param name="protocolVersion">The version of the protocol for the given channel.</param>
        /// <param name="address">The address of the given channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        public VersionedChannelInformation(Version protocolVersion, Uri address)
        {
            {
                Lokad.Enforce.Argument(() => protocolVersion);
                Lokad.Enforce.Argument(() => address);
            }

            m_ProtocolVersion = protocolVersion;
            m_Address = address;
        }

        /// <summary>
        /// Gets the version of the information object.
        /// </summary>
        public Version ProtocolVersion
        {
            get
            {
                return m_ProtocolVersion;
            }
        }

        /// <summary>
        /// Gets the address of the channel.
        /// </summary>
        public Uri Address
        {
            get
            {
                return m_Address;
            }
        }
    }
}