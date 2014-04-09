//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that is used to respond to a connection verification message.
    /// </summary>
    [DataContract]
    internal sealed class ConnectionVerificationResponseData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the connection verification response data.
        /// </summary>
        [DataMember]
        public object ResponseData
        {
            get;
            set;
        }
    }
}
