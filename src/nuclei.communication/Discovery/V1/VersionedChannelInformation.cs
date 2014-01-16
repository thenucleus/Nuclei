//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using ProtoBuf;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Stores information about one or more protocol channels.
    /// </summary>
    /// <remarks>
    /// This class should never be changed so that it is always backwards compatible.
    /// </remarks>
    [ProtoContract]
    internal sealed class VersionedChannelInformation
    {
        /// <summary>
        /// Gets the version of the information object.
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public Version ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the address of the channel.
        /// </summary>
        [ProtoMember(2, IsRequired = true)]
        public Uri Address
        {
            get;
            set;
        }
    }
}