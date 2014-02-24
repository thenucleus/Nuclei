//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// The default <see cref="ISerializeObjectData"/> implementation that simply passes the input object through as
    /// the serialized object.
    /// </summary>
    internal sealed class NonTransformingObjectSerializer : ISerializeObjectData
    {
        /// <summary>
        /// Gets the object type that the current serializer can serialize.
        /// </summary>
        public Type TypeToSerialize
        {
            get
            {
                return typeof(object);
            }
        }

        /// <summary>
        /// Turns the data provided by the input object into a serialized form that can be transmitted to the remote endpoint.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>An object that contains the serialized data.</returns>
        public object Serialize(object input)
        {
            return input;
        }

        /// <summary>
        /// Turns the serialized data into an object that can be used by the current endpoint.
        /// </summary>
        /// <param name="data">The serialized data.</param>
        /// <returns>An object that can be used by the current endpoint.</returns>
        public object Deserialize(object data)
        {
            return data;
        }
    }
}
