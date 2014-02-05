//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about a specific command, i.e. interface method.
    /// </summary>
    internal sealed class CommandData : IEquatable<CommandData>
    {
        /// <summary>
        /// The type of interface that provides the methods for the commands.
        /// </summary>
        private readonly Type m_InterfaceType;

        /// <summary>
        /// The name of the method that should be invoked when the command is executed.
        /// </summary>
        private readonly string m_MethodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData"/> class.
        /// </summary>
        /// <param name="interfaceType">The type of the interface that provides the methods for the commands.</param>
        /// <param name="methodName">The name of the method that should be invoked when the command is executed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="interfaceType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="methodName"/> is <see langword="null" />.
        /// </exception>
        public CommandData(Type interfaceType, string methodName)
        {
            {
                Lokad.Enforce.Argument(() => interfaceType);
                Lokad.Enforce.Argument(() => methodName);
                Lokad.Enforce.Argument(() => methodName, Lokad.Rules.StringIs.NotEmpty);
            }

            m_InterfaceType = interfaceType;
            m_MethodName = methodName;
        }

        /// <summary>
        /// Gets the type of the interface that provides the methods for the commands.
        /// </summary>
        public Type InterfaceType
        {
            [DebuggerStepThrough]
            get
            {
                return m_InterfaceType;
            }
        }

        /// <summary>
        /// Gets the name of the method that should be invoked when the command is executed.
        /// </summary>
        public string MethodName
        {
            [DebuggerStepThrough]
            get
            {
                return m_MethodName;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="CommandData"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="CommandData"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="CommandData"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(CommandData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && InterfaceType == other.InterfaceType 
                && string.Equals(MethodName, other.MethodName, StringComparison.OrdinalIgnoreCase);
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

            var id = obj as CommandData;
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
                hash = (hash * 23) ^ InterfaceType.GetHashCode();
                hash = (hash * 23) ^ MethodName.GetHashCode();

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
                "{0}.{1}",
                InterfaceType.FullName,
                MethodName);
        }
    }
}
