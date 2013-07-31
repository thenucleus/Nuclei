//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei
{
    /// <summary>
    /// Defines a method for comparing <see cref="Type"/> instances.
    /// </summary>
    public sealed class TypeComparer : IComparer<Type>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition
        /// Less than zero x is less than y.
        /// Zero x equals y.
        /// Greater than zero x is greater than y.
        /// </returns>
        public int Compare(Type x, Type y)
        {
            if ((x == null) && (y == null))
            {
                throw new ArgumentNullException("x");
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            // Compare by AssemblyName and FullName of the type
            return string.Compare(x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal);
        }
    }
}
