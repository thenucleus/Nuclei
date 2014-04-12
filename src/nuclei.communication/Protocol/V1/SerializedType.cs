//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Stores type information about a <see cref="Type"/>  in serializable form.
    /// </summary>
    [DataContract]
    internal sealed class SerializedType
    {
        /// <summary>
        /// Gets or sets the assembly name of the command set type.
        /// </summary>
        [DataMember]
        public string AssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full name of the type.
        /// </summary>
        [DataMember]
        public string FullName
        {
            get;
            set;
        }
    }
}
