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
    public sealed class UploadTokenTest : EqualityContractVerifierTest
    {
        private sealed class UploadTokenEqualityContractVerifier : EqualityContractVerifier<UploadToken>
        {
            private readonly UploadToken m_First = new UploadToken();

            private readonly UploadToken m_Second = new UploadToken();

            protected override UploadToken Copy(UploadToken original)
            {
                return original.Clone();
            }

            protected override UploadToken FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override UploadToken SecondInstance
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

        private sealed class UploadTokenHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<UploadToken> m_DistinctInstances
                = new List<UploadToken> 
                     {
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                        new UploadToken(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly UploadTokenHashcodeContractVerfier m_HashcodeVerifier = new UploadTokenHashcodeContractVerfier();

        private readonly UploadTokenEqualityContractVerifier m_EqualityVerifier = new UploadTokenEqualityContractVerifier();

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
            UploadToken first = null;
            UploadToken second = new UploadToken();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            UploadToken first = new UploadToken();
            UploadToken second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            UploadToken first = null;
            UploadToken second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new UploadToken();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new UploadToken(2);
            var second = new UploadToken(1);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new UploadToken(1);
            var second = new UploadToken(2);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            UploadToken first = null;
            UploadToken second = new UploadToken();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            UploadToken first = new UploadToken();
            UploadToken second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            UploadToken first = null;
            UploadToken second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new UploadToken();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new UploadToken(2);
            var second = new UploadToken(1);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new UploadToken(1);
            var second = new UploadToken(2);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            UploadToken first = new UploadToken();
            UploadToken second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            UploadToken first = new UploadToken();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new UploadToken();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new UploadToken(2);
            var second = new UploadToken(1);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new UploadToken(1);
            var second = new UploadToken(2);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            UploadToken first = new UploadToken();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
