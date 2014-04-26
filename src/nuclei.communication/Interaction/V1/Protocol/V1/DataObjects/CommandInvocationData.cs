//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has invoked a command.
    /// </summary>
    [DataContract]
    internal sealed class CommandInvocationData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the ID of the command that was invoked.
        /// </summary>
        [DataMember]
        public string CommandId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the array of method parameter types.
        /// </summary>
        [DataMember]
        public SerializedType[] ParameterTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the array of method parameter names.
        /// </summary>
        [DataMember]
        public string[] ParameterNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the array of method parameter values.
        /// </summary>
        [DataMember]
        public object[] ParameterValues
        {
            get;
            set;
        }
    }
}
