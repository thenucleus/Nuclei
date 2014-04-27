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

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationIdTest : EqualityContractVerifierTest
    {
        private sealed class NotificationIdEqualityContractVerifier : EqualityContractVerifier<NotificationId>
        {
            private readonly NotificationId m_First = new NotificationId("a");

            private readonly NotificationId m_Second = new NotificationId("b");

            protected override NotificationId Copy(NotificationId original)
            {
                return original.Clone();
            }

            protected override NotificationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override NotificationId SecondInstance
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

        private sealed class NotificationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<NotificationId> m_DistinctInstances
                = new List<NotificationId> 
                     {
                        new NotificationId("a"),
                        new NotificationId("b"),
                        new NotificationId("c"),
                        new NotificationId("d"),
                        new NotificationId("e"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly NotificationIdHashcodeContractVerfier m_HashcodeVerifier = new NotificationIdHashcodeContractVerfier();

        private readonly NotificationIdEqualityContractVerifier m_EqualityVerifier = new NotificationIdEqualityContractVerifier();

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
            NotificationId first = null;
            var second = new NotificationId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationId("a");
            NotificationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            NotificationId first = null;
            NotificationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new NotificationId("a");
            var second = new NotificationId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationId("b");
            var second = new NotificationId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationId("a");
            var second = new NotificationId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            NotificationId first = null;
            var second = new NotificationId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationId("a");
            NotificationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            NotificationId first = null;
            NotificationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new NotificationId("a");
            var second = new NotificationId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationId("b");
            var second = new NotificationId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationId("a");
            var second = new NotificationId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new NotificationId("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new NotificationId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new NotificationId("a");
            object second = new NotificationId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new NotificationId("b");
            object second = new NotificationId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new NotificationId("a");
            object second = new NotificationId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new NotificationId("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
