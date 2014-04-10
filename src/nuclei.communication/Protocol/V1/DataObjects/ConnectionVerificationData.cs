//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that is used to verify that a connection is still alive.
    /// </summary>
    [DataContract]
    internal sealed class ConnectionVerificationData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the type of the custom data.
        /// </summary>
        [DataMember]
        public SerializedType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom data for the verification message.
        /// </summary>
        [DataMember]
        public object CustomData
        {
            get;
            set;
        }
    }
}
