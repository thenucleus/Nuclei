//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nuclei.Communication
{
    /// <summary>
    /// Stores information regarding the protocol channels for a given endpoint.
    /// </summary>
    internal sealed class ProtocolInformation : IEquatable<ProtocolInformation>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ProtocolInformation first, ProtocolInformation second)
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
        public static bool operator !=(ProtocolInformation first, ProtocolInformation second)
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
        /// The version of the protocol for the current channel.
        /// </summary>
        private readonly Version m_Version;

        /// <summary>
        /// The address of the message channel for the given endpoint.
        /// </summary>
        private readonly Uri m_MessageAddress;

        /// <summary>
        /// The address of the data channel for the given endpoint.
        /// </summary>
        private readonly Uri m_DataAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolInformation"/> class.
        /// </summary>
        /// <param name="version">The version of the protocol for the given endpoint.</param>
        /// <param name="messageAddress">The address of the message channel for the given endpoint.</param>
        /// <param name="dataAddress">The address of the data channel for the given endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageAddress"/> is <see langword="null" />.
        /// </exception>
        public ProtocolInformation(Version version, Uri messageAddress, Uri dataAddress = null)
        {
            {
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => messageAddress);
            }

            m_Version = version;
            m_MessageAddress = messageAddress;
            m_DataAddress = dataAddress;
        }

        /// <summary>
        /// Gets the version of the information object.
        /// </summary>
        public Version Version
        {
            [DebuggerStepThrough]
            get
            {
                return m_Version;
            }
        }

        /// <summary>
        ///  Gets a value indicating the message URI of the channel.
        /// </summary>
        public Uri MessageAddress
        {
            [DebuggerStepThrough]
            get
            {
                return m_MessageAddress;
            }
        }

        /// <summary>
        /// Gets a value indicating the data URI of the channel.
        /// </summary>
        public Uri DataAddress
        {
            [DebuggerStepThrough]
            get
            {
                return m_DataAddress;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ProtocolInformation"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ProtocolInformation"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="ProtocolInformation"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ProtocolInformation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && Version.Equals(other.Version)
                && MessageAddress.Equals(other.MessageAddress);
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

            var id = obj as ProtocolInformation;
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
                hash = (hash * 23) ^ Version.GetHashCode();
                hash = (hash * 23) ^ MessageAddress.GetHashCode();

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
                "Protocol information. Version: {0}; Messages at: {1}; Data at: {2}",
                Version,
                MessageAddress,
                (DataAddress != null) ? DataAddress.ToString() : string.Empty);
        }
    }
}
