//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has executed a command
    /// and has gotten a response value.
    /// </summary>
    [DataContract]
    internal sealed class CommandInvocationResponseData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the actual <see cref="Type"/> of the returned value.
        /// </summary>
        public SerializedType ReturnedType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the result of the command execution.
        /// </summary>
        [DataMember]
        public object Result
        {
            get;
            set;
        }
    }
}
