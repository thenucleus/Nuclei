//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines what types of communication channels are available.
    /// </summary>
    [DataContract]
    public enum ChannelTemplate
    {
        /// <summary>
        /// The channel type is not defined.
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// The channel uses named pipes to communicate.
        /// </summary>
        [EnumMember]
        NamedPipe,

        /// <summary>
        /// The channel uses TCP/IP to communicate.
        /// </summary>
        [EnumMember]
        TcpIP,
    }
}
