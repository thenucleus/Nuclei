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

namespace Nuclei.Configuration
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ConfigurationKeyTest : EqualityContractVerifierTest
    {
        private sealed class MessageIdEqualityContractVerifier : EqualityContractVerifier<ConfigurationKey>
        {
            private readonly ConfigurationKey m_First
                = new ConfigurationKey("a", typeof(string));

            private readonly ConfigurationKey m_Second
                 = new ConfigurationKey("b", typeof(int));

            protected override ConfigurationKey Copy(ConfigurationKey original)
            {
                return new ConfigurationKey(original.Name, original.TranslateTo);
            }

            protected override ConfigurationKey FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ConfigurationKey SecondInstance
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
            private readonly IEnumerable<ConfigurationKey> m_DistinctInstances
                = new List<ConfigurationKey> 
                     {
                        new ConfigurationKey("a", typeof(string)),
                        new ConfigurationKey("b", typeof(int)),
                        new ConfigurationKey("c", typeof(double)),
                        new ConfigurationKey("d", typeof(float)),
                        new ConfigurationKey("e", typeof(string)),
                        new ConfigurationKey("f", typeof(Version)),
                        new ConfigurationKey("g", typeof(object)),
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

        [Test]
        public void Create()
        {
            var name = "a";
            var type = typeof(string);
            var key = new ConfigurationKey(name, type);

            Assert.AreEqual(name, key.Name);
            Assert.AreEqual(type, key.TranslateTo);
        }
    }
}
