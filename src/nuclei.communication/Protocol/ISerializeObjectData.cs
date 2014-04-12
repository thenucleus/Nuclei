//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that prepare object data for transmission to a remote endpoint.
    /// </summary>
    public interface ISerializeObjectData
    {
        /// <summary>
        /// Gets the object type that the current serializer can serialize.
        /// </summary>
        Type TypeToSerialize
        {
            get;
        }

        /// <summary>
        /// Turns the data provided by the input object into a serialized form that can be transmitted to the remote endpoint.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>An object that contains the serialized data.</returns>
        object Serialize(object input);

        /// <summary>
        /// Turns the serialized data into an object that can be used by the current endpoint.
        /// </summary>
        /// <param name="data">The serialized data.</param>
        /// <returns>An object that can be used by the current endpoint.</returns>
        object Deserialize(object data);
    }
}
