//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei
{
    /// <summary>
    /// Defines the base interface for ID numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Derivative classes should define the type parameters as:
    /// </para>
    /// <example>
    /// public sealed class SomeId : IIsId&lt;SomeId&gt;
    /// </example>
    /// </remarks>
    /// <typeparam name="TId">The type of the object which is the ID.</typeparam>
    public interface IIsId<TId> : IComparable<TId>, IComparable, IEquatable<TId>
        where TId : IIsId<TId>
    {
        /// <summary>
        /// Clones this ID number.
        /// </summary>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        TId Clone();
    }
}
