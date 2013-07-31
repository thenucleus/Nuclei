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

namespace Nuclei
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class IdTest : EqualityContractVerifierTest
    {
        private sealed class IdEqualityContractVerifier : EqualityContractVerifier<MockId>
        {
            private readonly MockId m_First = new MockId(0);

            private readonly MockId m_Second = new MockId(1);

            protected override MockId Copy(MockId original)
            {
                return original.Clone();
            }

            protected override MockId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MockId SecondInstance
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

        private sealed class IdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MockId> m_DistinctInstances
                = new List<MockId> 
                     {
                        new MockId(0),
                        new MockId(1),
                        new MockId(2),
                        new MockId(3),
                        new MockId(4),
                        new MockId(5),
                        new MockId(6),
                        new MockId(7),
                        new MockId(8),
                        new MockId(9),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly IdHashcodeContractVerfier m_HashcodeVerifier = new IdHashcodeContractVerfier();

        private readonly IdEqualityContractVerifier m_EqualityVerifier = new IdEqualityContractVerifier();

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
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            MockId first = null;
            MockId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            MockId first = new MockId(9);
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            MockId first = null;
            MockId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            MockId first = new MockId(9);
            MockId second = new MockId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            MockId first = new MockId(10);
            MockId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            MockId first = new MockId(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            object second = new MockId(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            MockId first = new MockId(11);
            object second = new MockId(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            MockId first = new MockId(10);
            object second = new MockId(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            MockId first = new MockId(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
