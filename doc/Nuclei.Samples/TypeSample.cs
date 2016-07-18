//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Samples
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TypeSample
    {
        [Test]
        public void IsAssignableToOpenGenericType()
        {
            {
                // Should return true
                var result = typeof(List<>).IsAssignableToOpenGenericType(typeof(List<int>));
                Assert.IsTrue(result);
            }

            {
                // Should return false
                var result = typeof(IList<>).IsAssignableToOpenGenericType(typeof(Dictionary<int, int>));
                Assert.IsFalse(result);
            }

            {
                // Should return true
                var result = typeof(IComparable<>).IsAssignableToOpenGenericType(typeof(bool));
                Assert.IsTrue(result);
            }
        }
    }
}
