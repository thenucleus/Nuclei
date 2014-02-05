//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication
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
        /// The channel uses a named pipe channel to communicate.
        /// </summary>
        [EnumMember]
        NamedPipe,

        /// <summary>
        /// The channel uses a TCP/IP channel to communicate.
        /// </summary>
        [EnumMember]
        TcpIP,

        /// <summary>
        /// The channel uses an HTTP connection to communicate.
        /// </summary>
        [EnumMember]
        Http,

        /// <summary>
        /// The channel uses an HTTPS connection to communicate.
        /// </summary>
        [EnumMember]
        Https,

        /// <summary>
        /// The channel uses an unknown method to communicate.
        /// </summary>
        [EnumMember]
        Unknown,
    }
}
