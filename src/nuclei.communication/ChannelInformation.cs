//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that store information regarding newly discovered
    /// endpoints.
    /// </summary>
    internal sealed class ChannelInformation
    {
        /// <summary>
        /// The ID of the endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// The version of the protocol for the current channel.
        /// </summary>
        private readonly Version m_ProtocolVersion;

        /// <summary>
        /// The address of the given channel.
        /// </summary>
        private readonly Uri m_Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="protocolVersion">The version of the protocol for the given channel.</param>
        /// <param name="address">The address of the given channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        public ChannelInformation(EndpointId id, Version protocolVersion, Uri address)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => protocolVersion);
                Lokad.Enforce.Argument(() => address);
            }

            m_Id = id;
            m_ProtocolVersion = protocolVersion;
            m_Address = address;
        }

        /// <summary>
        /// Gets the ID of the endpoint.
        /// </summary>
        public EndpointId Id
        {
            get
            {
                return m_Id;
            }
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
