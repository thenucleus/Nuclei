﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Contains information describing a data stream that is transferred between two endpoints.
    /// </summary>
    internal sealed class DataTransferMessage
    {
        /// <summary>
        /// Gets or sets the ID of the sending endpoint.
        /// </summary>
        public EndpointId SendingEndpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID of the receiving endpoint.
        /// </summary>
        public EndpointId ReceivingEndpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data stream.
        /// </summary>
        public Stream Data
        {
            get;
            set;
        }
    }
}