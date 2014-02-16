//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about a given command type.
    /// </summary>
    internal sealed class OfflineTypeInformation : IEquatable<OfflineTypeInformation>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(OfflineTypeInformation first, OfflineTypeInformation second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(OfflineTypeInformation first, OfflineTypeInformation second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// The full name of the type that defines the commands.
        /// </summary>
        private readonly string m_TypeFullName;

        /// <summary>
        /// The assembly information describing the assembly that contains the commands.
        /// </summary>
        private readonly AssemblyName m_AssemblyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineTypeInformation"/> class.
        /// </summary>
        /// <param name="typeFullName">The full name of the type that defines the commands.</param>
        /// <param name="assemblyName">The assembly information describing the assembly that contains the commands.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="typeFullName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblyName"/> is <see langword="null" />.
        /// </exception>
        public OfflineTypeInformation(string typeFullName, AssemblyName assemblyName)
        {
            {
                Lokad.Enforce.Argument(() => typeFullName);
                Lokad.Enforce.Argument(() => assemblyName);
            }

            m_TypeFullName = typeFullName;
            m_AssemblyName = assemblyName;
        }

        /// <summary>
        /// Gets the full name of the type that defines the commands.
        /// </summary>
        public string TypeFullName
        {
            [DebuggerStepThrough]
            get
            {
                return m_TypeFullName;
            }
        }

        /// <summary>
        /// Gets the assembly information describing the assembly that contains the commands.
        /// </summary>
        public AssemblyName AssemblyName
        {
            [DebuggerStepThrough]
            get
            {
                return m_AssemblyName;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the two offline types have the same 'contract', i.e. the
        /// same type name and assembly name, but not necessarily the same assembly version and strong name.
        /// </summary>
        /// <param name="other">The other type.</param>
        /// <returns>
        /// <see langword="true" /> if the current offline type has the same 'contract' as the other type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool AreSimilarContract(OfflineTypeInformation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && string.Equals(TypeFullName, other.TypeFullName, StringComparison.OrdinalIgnoreCase) 
                && AssemblyName.Name.Equals(other.AssemblyName.Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="OfflineTypeInformation"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="OfflineTypeInformation"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="OfflineTypeInformation"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(OfflineTypeInformation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && string.Equals(TypeFullName, other.TypeFullName, StringComparison.OrdinalIgnoreCase) 
                && AssemblyName.Equals(other.AssemblyName);
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

            var id = obj as OfflineTypeInformation;
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
                hash = (hash * 23) ^ m_TypeFullName.GetHashCode();
                hash = (hash * 23) ^ m_AssemblyName.GetHashCode();

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
                "{0}, {1}",
                m_TypeFullName,
                m_AssemblyName);
        }
    }
}
