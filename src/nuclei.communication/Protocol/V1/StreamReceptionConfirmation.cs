//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Stores data indicating whether a message was received correctly by the remote endpoint.
    /// </summary>
    [MessageContract]
    internal sealed class StreamReceptionConfirmation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the data stream was received correctly
        /// by the remote endpoint.
        /// </summary>
        [MessageHeader]
        public bool WasDataReceived
        {
            get;
            set;
        }
    }
}
