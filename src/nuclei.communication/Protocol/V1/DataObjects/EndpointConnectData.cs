//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has connected to
    /// the current endpoint.
    /// </summary>
    [DataContract]
    internal sealed class EndpointConnectData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the version of the discovery protocol used by the current endpoint.
        /// </summary>
        [DataMember]
        public Version DiscoveryVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the address of the discovery channel for the current endpoint.
        /// </summary>
        [DataMember]
        public Uri DiscoveryAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating what kind of channel was used
        /// to send this message.
        /// </summary>
        [DataMember]
        public ChannelTemplate ChannelTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the protocol that is used to connect to the current endpoint.
        /// </summary>
        [DataMember]
        public Version ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the URI of the channel that is used for message reception.
        /// </summary>
        [DataMember]
        public Uri MessageAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the URI of the channel that is used for data reception.
        /// </summary>
        [DataMember]
        public Uri DataAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the information describing the version of the communication protocol
        /// used by the sender, the desired communication API's for the sender and 
        /// the available communication API's provided by the sender.
        /// </summary>
        [DataMember]
        public CommunicationDescription Information
        {
            get;
            set;
        }
    }
}
