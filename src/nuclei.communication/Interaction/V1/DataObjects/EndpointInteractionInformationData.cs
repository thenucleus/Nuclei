//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has executed a command
    /// and has gotten a response value.
    /// </summary>
    [DataContract]
    internal sealed class EndpointInteractionInformationData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the collection containing the communication subject information for the endpoint.
        /// </summary>
        [DataMember]
        public SerializedSubjectInformation[] Groups
        {
            get;
            set;
        }
    }
}
