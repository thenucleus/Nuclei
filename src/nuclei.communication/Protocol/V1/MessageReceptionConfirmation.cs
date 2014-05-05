//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Stores data indicating whether a message was received correctly by the remote endpoint.
    /// </summary>
    [DataContract]
    internal sealed class MessageReceptionConfirmation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the message was received correctly
        /// by the remote endpoint.
        /// </summary>
        [DataMember]
        public bool WasDataReceived
        {
            get;
            set;
        }
    }
}
