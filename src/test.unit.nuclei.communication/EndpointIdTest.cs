//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointIdTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<EndpointId>
        {
            private readonly EndpointId m_First = new EndpointId("a");

            private readonly EndpointId m_Second = new EndpointId("b");

            protected override EndpointId Copy(EndpointId original)
            {
                return original.Clone();
            }

            protected override EndpointId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override EndpointId SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class EndpointIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<EndpointId> m_DistinctInstances
                = new List<EndpointId> 
                     {
                        new EndpointId("a"),
                        new EndpointId("b"),
                        new EndpointId("c"),
                        new EndpointId("d"),
                        new EndpointId("e"),
                        new EndpointId("f"),
                        new EndpointId("g"),
                        new EndpointId("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndpointIdHashcodeContractVerfier m_HashcodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier m_EqualityVerifier = new EndpointIdEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            EndpointId first = null;
            var second = new EndpointId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new EndpointId("a");
            EndpointId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            EndpointId first = null;
            EndpointId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new EndpointId("a");
            var second = new EndpointId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new EndpointId("b");
            var second = new EndpointId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new EndpointId("a");
            var second = new EndpointId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            EndpointId first = null;
            var second = new EndpointId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new EndpointId("a");
            EndpointId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            EndpointId first = null;
            EndpointId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new EndpointId("a");
            var second = new EndpointId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new EndpointId("b");
            var second = new EndpointId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new EndpointId("a");
            var second = new EndpointId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new EndpointId("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new EndpointId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new EndpointId("a");
            object second = new EndpointId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new EndpointId("b");
            object second = new EndpointId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new EndpointId("a");
            object second = new EndpointId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new EndpointId("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
