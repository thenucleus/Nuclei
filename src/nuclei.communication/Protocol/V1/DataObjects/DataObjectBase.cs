//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines the base object for objects that store data that should be send to a remote endpoint
    /// via the version 1.0 of the protocol layer.
    /// </summary>
    /// <design>
    /// Note that the <c>ProtoIncludeAttributes</c> need to be placed on this object because otherwise the
    /// protocol buffer compiler cannot figure out
    /// </design>
    [DataContract]
    internal abstract class DataObjectBase : IStoreV1CommunicationData
    {
        /// <summary>
        /// Gets or sets a value indicating the ID number of the message.
        /// </summary>
        [DataMember]
        public MessageId Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the ID number of the message to which 
        /// the current message is a response.
        /// </summary>
        [DataMember]
        public MessageId InResponseTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the ID number of the endpoint that 
        /// send the current message.
        /// </summary>
        [DataMember]
        public EndpointId Sender
        {
            get;
            set;
        }
    }
}
