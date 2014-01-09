//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Communication.Protocol;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommunicationSubjectTest : EqualityContractVerifierTest
    {
        private sealed class CommunicationSubjectEqualityContractVerifier : EqualityContractVerifier<CommunicationSubject>
        {
            private readonly CommunicationSubject m_First = new CommunicationSubject("a");

            private readonly CommunicationSubject m_Second = new CommunicationSubject("b");

            protected override CommunicationSubject Copy(CommunicationSubject original)
            {
                return original.Clone();
            }

            protected override CommunicationSubject FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override CommunicationSubject SecondInstance
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

        private sealed class CommunicationSubjectHashcodeContractVerifier : HashcodeContractVerifier
        {
            private readonly IEnumerable<CommunicationSubject> m_DistinctInstances
            = new List<CommunicationSubject> 
                     {
                        new CommunicationSubject("a"),
                        new CommunicationSubject("b"),
                        new CommunicationSubject("c"),
                        new CommunicationSubject("d"),
                        new CommunicationSubject("e"),
                        new CommunicationSubject("f"),
                        new CommunicationSubject("g"),
                        new CommunicationSubject("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly CommunicationSubjectHashcodeContractVerifier m_HashcodeVerifier = new CommunicationSubjectHashcodeContractVerifier();

        private readonly CommunicationSubjectEqualityContractVerifier m_EqualityVerifier = new CommunicationSubjectEqualityContractVerifier();

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
            CommunicationSubject first = null;
            var second = new CommunicationSubject("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new CommunicationSubject("a");
            CommunicationSubject second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            CommunicationSubject first = null;
            CommunicationSubject second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new CommunicationSubject("a");
            var second = new CommunicationSubject("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new CommunicationSubject("b");
            var second = new CommunicationSubject("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new CommunicationSubject("a");
            var second = new CommunicationSubject("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            CommunicationSubject first = null;
            var second = new CommunicationSubject("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new CommunicationSubject("a");
            CommunicationSubject second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            CommunicationSubject first = null;
            CommunicationSubject second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new CommunicationSubject("a");
            var second = new CommunicationSubject("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new CommunicationSubject("b");
            var second = new CommunicationSubject("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new CommunicationSubject("a");
            var second = new CommunicationSubject("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new CommunicationSubject("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new CommunicationSubject("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new CommunicationSubject("a");
            object second = new CommunicationSubject("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new CommunicationSubject("b");
            object second = new CommunicationSubject("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new CommunicationSubject("a");
            object second = new CommunicationSubject("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new CommunicationSubject("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
