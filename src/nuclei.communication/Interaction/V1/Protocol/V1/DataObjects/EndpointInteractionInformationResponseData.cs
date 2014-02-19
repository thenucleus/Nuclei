//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has received the interaction information 
    /// from the receiving endpoint and has determined the desired interaction level.
    /// </summary>
    [DataContract]
    internal sealed class EndpointInteractionInformationResponseData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the connection state.
        /// </summary>
        [DataMember]
        public string State
        {
            get;
            set;
        }
    }
}
