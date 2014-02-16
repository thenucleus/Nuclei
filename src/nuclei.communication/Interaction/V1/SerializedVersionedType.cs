//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Nuclei.Communication.Interaction.V1
{
    /// <summary>
    /// Stores a type and its version in serializable format.
    /// </summary>
    [DataContract]
    internal sealed class SerializedVersionedType
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public SerializedType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [DataMember]
        public Version Version
        {
            get;
            set;
        }
    }
}
