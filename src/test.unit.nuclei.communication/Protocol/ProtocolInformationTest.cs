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

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProtocolInformationTest : EqualityContractVerifierTest
    {
        private sealed class ProtocolInformationEqualityContractVerifier : EqualityContractVerifier<ProtocolInformation>
        {
            private readonly ProtocolInformation m_First 
                = new ProtocolInformation(
                    new Version(1, 0), 
                    new Uri("http://localhost/message/v1.0/invalid"));

            private readonly ProtocolInformation m_Second 
                = new ProtocolInformation(
                    new Version(2, 0), 
                    new Uri("http://localhost/message/v2.0/invalid"));

            protected override ProtocolInformation Copy(ProtocolInformation original)
            {
                return new ProtocolInformation(original.Version, original.MessageAddress, original.DataAddress);
            }

            protected override ProtocolInformation FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ProtocolInformation SecondInstance
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

        private sealed class ProtocolInformationHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ProtocolInformation> m_DistinctInstances
                = new List<ProtocolInformation> 
                     {
                        new ProtocolInformation(new Version(1, 0), new Uri("http://localhost/message/v1.0/invalid")),
                        new ProtocolInformation(new Version(1, 1), new Uri("http://localhost/message/v1.0/invalid")),
                        new ProtocolInformation(new Version(2, 0), new Uri("http://localhost/message/v1.0/invalid")),
                        new ProtocolInformation(new Version(1, 0), new Uri("http://localhost/message/v1.1/invalid")),
                        new ProtocolInformation(new Version(1, 0), new Uri("http://localhost/message/v2.0/invalid")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ProtocolInformationHashcodeContractVerfier m_HashcodeVerifier = new ProtocolInformationHashcodeContractVerfier();

        private readonly ProtocolInformationEqualityContractVerifier m_EqualityVerifier = new ProtocolInformationEqualityContractVerifier();

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
            var version = new Version(1, 2, 3, 4);
            var message = new Uri("http://localhost/message/invalid");
            var data = new Uri("http://localhost/data/invalid");
            var info = new ProtocolInformation(version, message, data);

            Assert.AreSame(version, info.Version);
            Assert.AreSame(message, info.MessageAddress);
            Assert.AreSame(data, info.DataAddress);
        }
    }
}
