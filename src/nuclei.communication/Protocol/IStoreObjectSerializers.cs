//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that store <see cref="ISerializeObjectData"/> instances.
    /// </summary>
    public interface IStoreObjectSerializers : IEnumerable<ISerializeObjectData>
    {
        /// <summary>
        /// Adds a new serializer.
        /// </summary>
        /// <param name="serializer">The serializer to add.</param>
        void Add(ISerializeObjectData serializer);

        /// <summary>
        /// Returns a value indicating whether a serializer for the given type exists.
        /// </summary>
        /// <param name="type">The type to be serialized.</param>
        /// <returns>
        /// <see langword="true" /> if a serializer for the given type exists; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasSerializerFor(Type type);

        /// <summary>
        /// Returns the serializer for the given type.
        /// </summary>
        /// <param name="type">The type to be serialized.</param>
        /// <returns>The serializer for the given type.</returns>
        ISerializeObjectData SerializerFor(Type type);
    }
}
