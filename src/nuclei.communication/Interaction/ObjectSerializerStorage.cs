//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Nuclei.Communication.Properties;
using QuickGraph;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores <see cref="ISerializeObjectData"/> instances.
    /// </summary>
    internal sealed class ObjectSerializerStorage : IStoreObjectSerializers
    {
        /// <summary>
        /// Maps a type to a flag indicating whether or not a serializer exists for the given type.
        /// </summary>
        private sealed class TypeMap : IEquatable<TypeMap>
        {
            /// <summary>
            /// The type.
            /// </summary>
            private readonly Type m_Type;

            /// <summary>
            /// A flag that indicates whether or not a serializer for the given type exists.
            /// </summary>
            private bool m_HasSerializer;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeMap"/> class.
            /// </summary>
            /// <param name="type">The type for the map.</param>
            public TypeMap(Type type)
            {
                {
                    Debug.Assert(type != null, "The type for the type map should not be a null reference.");
                }

                m_Type = type;
            }

            /// <summary>
            /// Gets the type for the map.
            /// </summary>
            public Type Type
            {
                [DebuggerStepThrough]
                get
                {
                    return m_Type;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether a serializer exists for the given type.
            /// </summary>
            public bool HasSerializer
            {
                [DebuggerStepThrough]
                get
                {
                    return m_HasSerializer;
                }

                [DebuggerStepThrough]
                set
                {
                    m_HasSerializer = value;
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="TypeMap"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="TypeMap"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="TypeMap"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(TypeMap other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                return !ReferenceEquals(other, null) && m_Type.Equals(other.m_Type);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var id = obj as TypeMap;
                return Equals(id);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                // As obtained from the Jon Skeet answer to:
                // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    hash = (hash * 23) ^ m_Type.GetHashCode();

                    return hash;
                }
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "TypeMap [{0}]",
                    m_Type);
            }
        }

        /// <summary>
        /// The collection that holds all the known serializers.
        /// </summary>
        private readonly Dictionary<Type, ISerializeObjectData> m_Serializers
            = new Dictionary<Type, ISerializeObjectData>();

        /// <summary>
        /// The type graph that maps each type to its parent types.
        /// </summary>
        private readonly BidirectionalGraph<TypeMap, Edge<TypeMap>> m_TypeGraph
            = new BidirectionalGraph<TypeMap, Edge<TypeMap>>(false);

        /// <summary>
        /// Adds a new serializer.
        /// </summary>
        /// <param name="serializer">The serializer to add.</param>
        public void Add(ISerializeObjectData serializer)
        {
            {
                Lokad.Enforce.Argument(() => serializer);
                Lokad.Enforce.With<DuplicateObjectSerializerException>(
                    !m_Serializers.ContainsKey(serializer.TypeToSerialize),
                    Resources.Exceptions_Messages_DuplicateObjectSerializer);
            }

            m_Serializers.Add(serializer.TypeToSerialize, serializer);
            AddSerializerTypeToGraph(serializer.TypeToSerialize);
        }

        private void AddSerializerTypeToGraph(Type type)
        {
            var vertex = TypeMapFor(type);
            if (vertex != null)
            {
                vertex.HasSerializer = true;
                return;
            }

            var map = AddTypeToGraph(type);
            map.HasSerializer = true;
        }

        private TypeMap TypeMapFor(Type type)
        {
            return m_TypeGraph.Vertices.FirstOrDefault(m => m.Type.Equals(type));
        }

        private TypeMap AddTypeToGraph(Type type)
        {
            var map = new TypeMap(type);
            m_TypeGraph.AddVertex(map);
            if ((type.BaseType != null) && (!type.BaseType.Equals(type)))
            {
                var baseMap = AddTypeToGraph(type.BaseType);
                m_TypeGraph.AddEdge(new Edge<TypeMap>(map, baseMap));
            }

            if (type.IsGenericType && !type.IsGenericTypeDefinition && (type.GetGenericTypeDefinition() != null))
            {
                var genericBaseType = type.GetGenericTypeDefinition();
                if (genericBaseType != type)
                {
                    var definitionMap = AddTypeToGraph(genericBaseType);
                    m_TypeGraph.AddEdge(new Edge<TypeMap>(map, definitionMap));
                }
            }

            foreach (var baseInterface in type.GetInterfaces())
            {
                if (baseInterface != null)
                {
                    var interfaceMap = AddTypeToGraph(baseInterface);
                    m_TypeGraph.AddEdge(new Edge<TypeMap>(map, interfaceMap));
                }
            }

            return map;
        }

        /// <summary>
        /// Returns a value indicating whether a serializer for the given type exists.
        /// </summary>
        /// <param name="type">The type to be serialized.</param>
        /// <returns>
        /// <see langword="true" /> if a serializer for the given type exists; otherwise, <see langword="false" />.
        /// </returns>
        public bool HasSerializerFor(Type type)
        {
            if (type == null)
            {
                return false;
            }

            var serializer = MostSuitableSerializerFor(type);
            return serializer != null;
        }

        private ISerializeObjectData MostSuitableSerializerFor(Type type)
        {
            if (m_Serializers.Count == 0)
            {
                return null;
            }

            if (m_Serializers.ContainsKey(type))
            {
                return m_Serializers[type];
            }

            var map = FindClosestMapMatchingPredicate(type, m => m.HasSerializer);
            if (map != null)
            {
                return m_Serializers[map.Type];
            }

            return null;
        }

        private TypeMap FindClosestMapMatchingPredicate(Type type, Predicate<TypeMap> selector)
        {
            var typeQueue = new Queue<Type>();
            typeQueue.Enqueue(type);
            while (typeQueue.Count > 0)
            {
                var queuedType = typeQueue.Dequeue();
                var directMap = TypeMapFor(queuedType);
                if ((directMap != null) && selector(directMap))
                {
                    return directMap;
                }

                if ((queuedType.BaseType != null) && (!queuedType.BaseType.Equals(queuedType)))
                {
                    typeQueue.Enqueue(queuedType.BaseType);
                }

                if (queuedType.IsGenericType && !queuedType.IsGenericTypeDefinition && (queuedType.GetGenericTypeDefinition() != null))
                {
                    typeQueue.Enqueue(queuedType.GetGenericTypeDefinition());
                }

                foreach (var baseInterface in queuedType.GetInterfaces())
                {
                    if (baseInterface != null)
                    {
                        typeQueue.Enqueue(baseInterface);
                    }
                }
            }

            // Technically this shouldn't happen unless we have no map for System.Object
            return null;
        }

        /// <summary>
        /// Returns the serializer for the given type.
        /// </summary>
        /// <param name="type">The type to be serialized.</param>
        /// <returns>The serializer for the given type.</returns>
        public ISerializeObjectData SerializerFor(Type type)
        {
            {
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.With<NoSerializerForTypeFoundException>(
                    HasSerializerFor(type),
                    Resources.Exceptions_Messages_NoSerializerForTypeFound);
            }

            return MostSuitableSerializerFor(type);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ISerializeObjectData> GetEnumerator()
        {
            return m_Serializers.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
