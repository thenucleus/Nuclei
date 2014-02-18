//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the order of preference for a set of commands or notifications.
    /// </summary>
    internal sealed class VersionedTypeFallback : IEnumerable<Tuple<OfflineTypeInformation, Version>>
    {
        private sealed class VersionedOfflineTypeEqualityComparer : IEqualityComparer<Tuple<OfflineTypeInformation, Version>>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(Tuple<OfflineTypeInformation, Version> x, Tuple<OfflineTypeInformation, Version> y)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                return x.Item1.AreSimilarContract(y.Item1);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
            /// </exception>
            public int GetHashCode(Tuple<OfflineTypeInformation, Version> obj)
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
                    hash = (hash * 23) ^ obj.Item1.GetHashCode();
                    hash = (hash * 23) ^ obj.Item2.GetHashCode();

                    return hash;
                }
            }
        }

        /// <summary>
        /// The collection containing all the types that provide a single capability.
        /// </summary>
        private readonly List<Tuple<OfflineTypeInformation, Version>> m_Types
            = new List<Tuple<OfflineTypeInformation, Version>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedTypeFallback"/> class.
        /// </summary>
        /// <param name="versionedTypes">The collection containing all the types that provide a single capability.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="versionedTypes"/> is <see langword="null" />.
        /// </exception>
        public VersionedTypeFallback(params Tuple<OfflineTypeInformation, Version>[] versionedTypes)
        {
            {
                Lokad.Enforce.Argument(() => versionedTypes);
            }

            m_Types.AddRange(versionedTypes);
        }

        /// <summary>
        /// Returns a value indicating whether there is at least one type that is in both collections.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>
        /// <see langword="true" /> if there is at least one type that exists in both collections; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsPartialMatch(VersionedTypeFallback other)
        {
            return m_Types.Intersect(other.m_Types, new VersionedOfflineTypeEqualityComparer()).Any();
        }

        /// <summary>
        /// Returns the type information for the type which is in both collections and has the highest version number.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>
        /// The type information for the type which is in both collections and has the highest version number.
        /// </returns>
        public OfflineTypeInformation HighestVersionMatch(VersionedTypeFallback other)
        {
            var bestMatchPair = m_Types.Intersect(other.m_Types, new VersionedOfflineTypeEqualityComparer())
                .OrderBy(t => t.Item2)
                .FirstOrDefault();

            return bestMatchPair != null ? bestMatchPair.Item1 : null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Tuple<OfflineTypeInformation, Version>> GetEnumerator()
        {
            return m_Types.GetEnumerator();
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