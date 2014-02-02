//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction.Transport.V1
{
    /// <summary>
    /// Defines the interface for objects that carry information, in serialized form, about 
    /// a specific <see cref="ICommandSet"/>.
    /// </summary>
    internal interface ISerializedType : IEquatable<ISerializedType>
    {
        /// <summary>
        /// Gets the assembly qualified name of the command set type.
        /// </summary>
        string AssemblyQualifiedTypeName
        {
            get;
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        string FullName
        {
            get;
        }
    }
}
