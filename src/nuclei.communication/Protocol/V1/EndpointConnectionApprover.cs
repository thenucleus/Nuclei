//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Approves connections with remote endpoints for endpoints that use V1 of the communication protocol.
    /// </summary>
    internal sealed class EndpointConnectionApprover : IApproveEndpointConnections
    {
        /// <summary>
        /// The object that stores information about the available communication APIs.
        /// </summary>
        private readonly IStoreCommunicationDescriptions m_Descriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConnectionApprover"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="descriptions"/> is <see langword="null" />.
        /// </exception>
        public EndpointConnectionApprover(IStoreCommunicationDescriptions descriptions)
        {
            {
                Lokad.Enforce.Argument(() => descriptions);
            }

            m_Descriptions = descriptions;
        }

        /// <summary>
        /// The version of the protocol for which the current instance can approve endpoint connections.
        /// </summary>
        public Version ProtocolVersion
        {
            get
            {
                return ProtocolVersions.V1;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given remote endpoint is allowed to connect to the
        /// current endpoint.
        /// </summary>
        /// <param name="information">The connection description for the remote endpoint.</param>
        /// <returns>
        /// <see langword="true"/> if the remote endpoint is allowed to connect to the current endpoint; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsEndpointAllowedToConnect(CommunicationDescription information)
        {
            return information.Subjects.Intersect(m_Descriptions.Subjects()).Any();
        }
    }
}
