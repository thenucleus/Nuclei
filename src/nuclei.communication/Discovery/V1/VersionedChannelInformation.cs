//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Stores information about one or more protocol channels.
    /// </summary>
    /// <remarks>
    /// This class should never be changed so that it is always backwards compatible.
    /// </remarks>
    [DataContract]
    internal sealed class VersionedChannelInformation
    {
        /// <summary>
        /// Gets or sets the version of the information object.
        /// </summary>
        [DataMember]
        public Version ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the address of the channel.
        /// </summary>
        [DataMember]
        public Uri Address
        {
            get;
            set;
        }
    }
}
