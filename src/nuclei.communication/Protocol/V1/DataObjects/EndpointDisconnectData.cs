//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint is about to disconnect
    /// from the network.
    /// </summary>
    [DataContract]
    internal sealed class EndpointDisconnectData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets a value indicating why the channel is being closed.
        /// </summary>
        [DataMember]
        public string DisconnectReason
        {
            get;
            set;
        }
    }
}
