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

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageIdTest : EqualityContractVerifierTest
    {
        private sealed class MessageIdEqualityContractVerifier : EqualityContractVerifier<MessageId>
        {
            private readonly MessageId m_First = new MessageId();

            private readonly MessageId m_Second = new MessageId();

            protected override MessageId Copy(MessageId original)
            {
                return original.Clone();
            }

            protected override MessageId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MessageId SecondInstance
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

        private sealed class MessageIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MessageId> m_DistinctInstances
                = new List<MessageId> 
                     {
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                        new MessageId(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MessageIdHashcodeContractVerfier m_HashcodeVerifier = new MessageIdHashcodeContractVerfier();

        private readonly MessageIdEqualityContractVerifier m_EqualityVerifier = new MessageIdEqualityContractVerifier();

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

        private static MessageId Create(Guid id)
        {
            var type = typeof(MessageId);
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                    {
                        typeof(Guid)
                    },
                null);

            return (MessageId)constructor.Invoke(new object[] { id });
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            MessageId first = null;
            MessageId second = new MessageId();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            MessageId first = new MessageId();
            MessageId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            MessageId first = null;
            MessageId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new MessageId();
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
            MessageId first = null;
            MessageId second = new MessageId();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            MessageId first = new MessageId();
            MessageId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            MessageId first = null;
            MessageId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new MessageId();
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
            MessageId first = new MessageId();
            MessageId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            MessageId first = new MessageId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new MessageId();
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
            MessageId first = new MessageId();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
