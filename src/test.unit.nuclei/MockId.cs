﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace Nuclei
{
    /// <summary>
    /// Defines a mock ID number class, used in testing.
    /// </summary>
    public sealed class MockId : Id<MockId, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MockId(int value)
            : base(value)
        {
        }

        /// <summary>
        /// Clones the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new <see cref="MockId"/> with the given value as internal ID.</returns>
        protected override MockId Clone(int value)
        {
            return new MockId(value);
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
