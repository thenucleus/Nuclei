//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines what types of communication channels are available.
    /// </summary>
    public enum ChannelTemplate
    {
        /// <summary>
        /// The channel type is not defined.
        /// </summary>
        None,

        /// <summary>
        /// The channel uses named pipes to communicate.
        /// </summary>
        NamedPipe,

        /// <summary>
        /// The channel uses TCP/IP to communicate.
        /// </summary>
        TcpIP,
    }
}
