//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Interaction.V1
{
    /// <summary>
    /// Stores a set of command or notification types ordered by version.
    /// </summary>
    [DataContract]
    internal sealed class SerializedTypeFallback
    {
        /// <summary>
        /// Gets or sets the versioned types for the current fallback group.
        /// </summary>
        [DataMember]
        public SerializedVersionedType[] Types
        {
            get;
            set;
        }
    }
}