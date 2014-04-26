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
    public sealed class CommandIdTest : EqualityContractVerifierTest
    {
        private sealed class CommandIdEqualityContractVerifier : EqualityContractVerifier<CommandId>
        {
            private readonly CommandId m_First = CommandId.Create(typeof(string).GetMethod("CompareTo", new[] { typeof(object) }));

            private readonly CommandId m_Second = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            protected override CommandId Copy(CommandId original)
            {
                return original.Clone();
            }

            protected override CommandId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override CommandId SecondInstance
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

        private sealed class CommandIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<CommandId> m_DistinctInstances
                = new List<CommandId> 
                     {
                        CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) })),
                        CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) })),
                        CommandId.Create(typeof(string).GetMethod("CompareTo", new[] { typeof(object) })),
                        CommandId.Create(typeof(string).GetMethod("Equals", new[] { typeof(object) })),
                        CommandId.Create(typeof(object).GetMethod("Equals", new[] { typeof(object) })),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly CommandIdHashcodeContractVerfier m_HashcodeVerifier = new CommandIdHashcodeContractVerfier();

        private readonly CommandIdEqualityContractVerifier m_EqualityVerifier = new CommandIdEqualityContractVerifier();

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
            CommandId first = null;
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            CommandId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            CommandId first = null;
            CommandId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            CommandId first = null;
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            CommandId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            CommandId first = null;
            CommandId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            object second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));
            object second = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            object second = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = CommandId.Create(typeof(double).GetMethod("CompareTo", new[] { typeof(object) }));
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
