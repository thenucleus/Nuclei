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
    /// an event.
    /// </summary>
    internal interface ISerializedEventRegistration : IEquatable<ISerializedEventRegistration>
    {
        /// <summary>
        /// Gets the command set on which the method was invoked.
        /// </summary>
        ISerializedType Type
        {
            get;
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        string MemberName
        {
            get;
        }
    }
}
