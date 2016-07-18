//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace Nuclei.Samples
{
    /// <summary>
    /// Defines an ID that uses a string as the internal identifying value.
    /// </summary>
    public sealed class SampleId : Id<SampleId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SampleId(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Clones the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new <see cref="SampleId"/> with the given value as internal ID.</returns>
        protected override SampleId Clone(string value)
        {
            return new SampleId(value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "ID: {0}", InternalValue);
        }
    }
}
