//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Interaction.Transport.V1
{
    /// <summary>
    /// Stores information about a serialized method invocation.
    /// </summary>
    [Serializable]
    internal sealed class SerializedMethodInvocation : ISerializedMethodInvocation
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedMethodInvocation first, SerializedMethodInvocation second)
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
        public static bool operator !=(SerializedMethodInvocation first, SerializedMethodInvocation second)
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
        /// The collection that holds the parameter names and values.
        /// </summary>
        private readonly List<Tuple<ISerializedType, object>> m_Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedMethodInvocation"/> class.
        /// </summary>
        /// <param name="type">The serialized information about the command set.</param>
        /// <param name="methodName">The name of the method that was called.</param>
        /// <param name="namedParameters">The collection of parameter names and values.</param>
        public SerializedMethodInvocation(ISerializedType type, string methodName, List<Tuple<ISerializedType, object>> namedParameters)
        {
            {
                Debug.Assert(type != null, "No type information specified.");
                Debug.Assert(!string.IsNullOrWhiteSpace(methodName), "No method name specified.");
            }

            Type = type;
            MemberName = methodName;
            m_Parameters = namedParameters;
        }

        /// <summary>
        /// Gets the command set on which the method was invoked.
        /// </summary>
        public ISerializedType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string MemberName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection which contains the names and values of the parameters.
        /// </summary>
        public List<Tuple<ISerializedType, object>> Parameters
        {
            get
            {
                return m_Parameters;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the other parameter; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ISerializedMethodInvocation other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var sameType = Type.Equals(other.Type)
                && string.Equals(MemberName, other.MemberName, StringComparison.Ordinal);

            var sameParameters = Parameters.Count == other.Parameters.Count;
            if (sameParameters)
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    var ours = Parameters[i];
                    var theirs = other.Parameters[i];

                    sameParameters = ours.Item1.Equals(theirs.Item1) && (ours.Item2 == theirs.Item2);
                    if (!sameParameters)
                    {
                        break;
                    }
                }
            }

            return sameType && sameParameters;
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as ISerializedMethodInvocation;
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
                hash = (hash * 23) ^ Type.GetHashCode();
                hash = (hash * 23) ^ MemberName.GetHashCode();
                foreach (var pair in Parameters)
                {
                    hash = (hash * 23) ^ pair.Item1.GetHashCode();
                    hash = (hash * 23) ^ pair.Item2.GetHashCode();
                }

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
            return Type.FullName + "." + MemberName;
        }
    }
}
