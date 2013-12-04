//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class TimingGroupTest : EqualityContractVerifierTest
    {
        private sealed class TimingGroupEqualityContractVerifier : EqualityContractVerifier<TimingGroup>
        {
            private readonly TimingGroup m_First = new TimingGroup();

            private readonly TimingGroup m_Second = new TimingGroup();

            protected override TimingGroup Copy(TimingGroup original)
            {
                return original.Clone();
            }

            protected override TimingGroup FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override TimingGroup SecondInstance
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

        private sealed class TimingGroupHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<TimingGroup> m_DistinctInstances
                = new List<TimingGroup> 
                     {
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                        new TimingGroup(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly TimingGroupHashcodeContractVerfier m_HashcodeVerifier = new TimingGroupHashcodeContractVerfier();

        private readonly TimingGroupEqualityContractVerifier m_EqualityVerifier = new TimingGroupEqualityContractVerifier();

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

        private static TimingGroup Create(Guid id)
        {
            var type = typeof(TimingGroup);
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                    {
                        typeof(Guid)
                    },
                null);

            return (TimingGroup)constructor.Invoke(new object[] { id });
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            TimingGroup first = null;
            TimingGroup second = new TimingGroup();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            TimingGroup first = new TimingGroup();
            TimingGroup second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            TimingGroup first = null;
            TimingGroup second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new TimingGroup();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            TimingGroup first = null;
            TimingGroup second = new TimingGroup();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            TimingGroup first = new TimingGroup();
            TimingGroup second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            TimingGroup first = null;
            TimingGroup second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new TimingGroup();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            TimingGroup first = new TimingGroup();
            TimingGroup second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            TimingGroup first = new TimingGroup();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new TimingGroup();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            TimingGroup first = new TimingGroup();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
