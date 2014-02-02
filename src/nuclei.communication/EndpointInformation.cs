//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

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
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
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
        /// The object describing the discovery channel for the endpoint.
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
        /// The object describing the protocol information for the endpoint.
        /// </summary>
        public ProtocolInformation ProtocolInformation
        {
            [DebuggerStepThrough]
            get
            {
                return m_ProtocolInformation;
            }
        }

        public InteractionInformation InteractionInformation
        {
            [DebuggerStepThrough]
            get
            {
                return m_InteractionInformation;
            }
        }
    }
}
