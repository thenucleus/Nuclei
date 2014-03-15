//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class OfflineTypeInformationTest : EqualityContractVerifierTest
    {
        private sealed class OfflineTypeInformationEqualityContractVerifier : EqualityContractVerifier<OfflineTypeInformation>
        {
            private readonly OfflineTypeInformation m_First = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());

            private readonly OfflineTypeInformation m_Second = new OfflineTypeInformation(typeof(string).FullName, typeof(string).Assembly.GetName());

            protected override OfflineTypeInformation Copy(OfflineTypeInformation original)
            {
                return new OfflineTypeInformation(original.TypeFullName, original.AssemblyName);
            }

            protected override OfflineTypeInformation FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override OfflineTypeInformation SecondInstance
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

        private sealed class OfflineTypeInformationHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<OfflineTypeInformation> m_DistinctInstances
                = new List<OfflineTypeInformation> 
                     {
                        new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()),
                        new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()),
                        new OfflineTypeInformation(typeof(object).FullName, typeof(object).Assembly.GetName()),
                        new OfflineTypeInformation(typeof(OfflineTypeInformation).FullName, typeof(OfflineTypeInformation).Assembly.GetName()),
                        new OfflineTypeInformation(typeof(TestAttribute).FullName, typeof(TestAttribute).Assembly.GetName()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly OfflineTypeInformationHashcodeContractVerfier m_HashcodeVerifier = new OfflineTypeInformationHashcodeContractVerfier();

        private readonly OfflineTypeInformationEqualityContractVerifier m_EqualityVerifier = new OfflineTypeInformationEqualityContractVerifier();

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
        public void Create()
        {
            var type = typeof(int);
            var data = new OfflineTypeInformation(type.FullName, type.Assembly.GetName());

            Assert.AreSame(type.FullName, data.TypeFullName);
            Assert.AreEqual(type.Assembly.GetName(), data.AssemblyName);
        }

        [Test]
        public void AreSimilarContractWithNullReference()
        {
            var type = typeof(int);
            var data = new OfflineTypeInformation(type.FullName, type.Assembly.GetName());
            Assert.IsFalse(data.AreSimilarContract(null));
        }

        [Test]
        public void AreSimilarContractWithNonSimilarTypes()
        {
            var first = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new OfflineTypeInformation(typeof(TestAttribute).FullName, typeof(TestAttribute).Assembly.GetName());
            Assert.IsFalse(first.AreSimilarContract(second));
        }

        [Test]
        public void AreSimilarContractWithIdenticalTypes()
        {
            var first = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            Assert.IsTrue(first.AreSimilarContract(second));
        }

        [Test]
        public void AreSimilarContractWithSimilarTypes()
        {
            var first = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new OfflineTypeInformation(typeof(int).FullName, new AssemblyName(typeof(int).Assembly.GetName().Name));
            Assert.IsTrue(first.AreSimilarContract(second));
        }

        [Test]
        public void AreSimilarContractWithSimilarTypesInDifferentAssemblies()
        {
            var first = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new OfflineTypeInformation(typeof(int).FullName, new AssemblyName(typeof(TestAttribute).Assembly.GetName().Name));
            Assert.IsFalse(first.AreSimilarContract(second));
        }
    }
}
