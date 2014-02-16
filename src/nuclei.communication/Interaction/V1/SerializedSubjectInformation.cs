//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Interaction.V1
{
    /// <summary>
    /// Stores serializable information about an <see cref="CommunicationSubjectGroup"/>.
    /// </summary>
    [DataContract]
    internal sealed class SerializedSubjectInformation
    {
        /// <summary>
        /// Gets or sets the subject of the group.
        /// </summary>
        [DataMember]
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of commands related to the group.
        /// </summary>
        [DataMember]
        public SerializedTypeFallback[] Commands
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of notifications related to the group.
        /// </summary>
        [DataMember]
        public SerializedTypeFallback[] Notifications
        {
            get;
            set;
        }
    }
}
