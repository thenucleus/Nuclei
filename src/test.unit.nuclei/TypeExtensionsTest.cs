//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeExtensionsTest
    {
        [Test]
        public void IsAssignableToOpenGenericTypeWithGenericType()
        {
            Assert.IsTrue(typeof(List<>).IsAssignableToOpenGenericType(typeof(List<int>)));

            Assert.IsTrue(typeof(IEnumerable<>).IsAssignableToOpenGenericType(typeof(IEnumerable<int>)));
            Assert.IsTrue(typeof(IEnumerable<>).IsAssignableToOpenGenericType(typeof(IList<int>)));
            Assert.IsTrue(typeof(IEnumerable<>).IsAssignableToOpenGenericType(typeof(List<int>)));

            Assert.IsTrue(typeof(IComparable<>).IsAssignableToOpenGenericType(typeof(int)));

            Assert.IsFalse(typeof(List<>).IsAssignableToOpenGenericType(typeof(int)));
            Assert.IsFalse(typeof(IEnumerable<>).IsAssignableToOpenGenericType(typeof(object)));
        }

        [Test]
        public void IsAssignableToOpenGenericTypeWithNonGenericType()
        {
            Assert.IsFalse(typeof(List<>).IsAssignableToOpenGenericType(typeof(ArrayList)));
        }
    }
}
