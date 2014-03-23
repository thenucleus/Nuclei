//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.ServiceModel;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Contains information describing a data stream that is transferred between two endpoints.
    /// </summary>
    [MessageContract]
    internal sealed class StreamData
    {
        /// <summary>
        /// Gets or sets the ID of the sending endpoint.
        /// </summary>
        [MessageHeader]
        public EndpointId SendingEndpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data stream.
        /// </summary>
        [MessageBodyMember]
        public Stream Data
        {
            get;
            set;
        }
    }
}
