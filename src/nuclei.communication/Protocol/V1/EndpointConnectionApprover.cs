using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclei.Communication.Protocol.V1
{
    internal sealed class EndpointConnectionApprover : IApproveEndpointConnections
    {
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
