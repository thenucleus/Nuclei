//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines an ID number for groups of timing intervals.
    /// </summary>
    [Serializable]
    public sealed class TimingGroup : Id<TimingGroup, Guid>
    {
        /// <summary>
        /// Returns a new Guid.
        /// </summary>
        /// <returns>A new Guid.</returns>
        private static Guid Next()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingGroup"/> class.
        /// </summary>
        public TimingGroup()
            : this(Next())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingGroup"/> class.
        /// </summary>
        /// <param name="id">The Guid for the ID.</param>
        private TimingGroup(Guid id)
            : base(id)
        { 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override TimingGroup Clone(Guid value)
        {
            return new TimingGroup(value);
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
                "IntervalGroup: [{0}]",
                InternalValue);
        }
    }
}
