//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that store information regarding newly discovered
    /// endpoints.
    /// </summary>
    internal sealed class EndpointInformation
    {
        /// <summary>
        /// The ID of the endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// The object describing the discovery channel for the endpoint.
        /// </summary>
        private readonly DiscoveryInformation m_DiscoveryInformation;

        /// <summary>
        /// The object describing the protocol information for the endpoint.
        /// </summary>
        private readonly ProtocolInformation m_ProtocolInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInformation"/> class.
        /// </summary>
        /// <param name="id">The endpoint ID of the endpoint which is being described by the current information instance.</param>
        /// <param name="discoveryInformation">The discovery information for the endpoint.</param>
        /// <param name="protocolInformation">The protocol information for the endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoveryInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolInformation"/> is <see langword="null" />.
        /// </exception>
        public EndpointInformation(EndpointId id, DiscoveryInformation discoveryInformation, ProtocolInformation protocolInformation)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => discoveryInformation);
                Lokad.Enforce.Argument(() => protocolInformation);
            }

            m_Id = id;
            m_DiscoveryInformation = discoveryInformation;
            m_ProtocolInformation = protocolInformation;
        }

        /// <summary>
        /// Gets the ID of the endpoint.
        /// </summary>
        public EndpointId Id
        {
            [DebuggerStepThrough]
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the object describing the discovery channel for the endpoint.
        /// </summary>
        public DiscoveryInformation DiscoveryInformation
        {
            [DebuggerStepThrough]
            get
            {
                return m_DiscoveryInformation;
            }
        }

        /// <summary>
        /// Gets the object describing the protocol information for the endpoint.
        /// </summary>
        public ProtocolInformation ProtocolInformation
        {
            [DebuggerStepThrough]
            get
            {
                return m_ProtocolInformation;
            }
        }
    }
}
