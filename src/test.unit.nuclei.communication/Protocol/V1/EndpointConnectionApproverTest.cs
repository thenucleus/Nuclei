//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectionApproverTest
    {
        [Test]
        public void IsEndpointAllowedToConnectWithApprovedEndpoint()
        {
            var subjects = new List<CommunicationSubject>
                {
                    new CommunicationSubject("a"),
                    new CommunicationSubject("b")
                };
            var storage = new Mock<IStoreProtocolSubjects>();
            {
                storage.Setup(s => s.Subjects())
                    .Returns(subjects);
            }

            var approver = new EndpointConnectionApprover(storage.Object);

            var description = new CommunicationDescription(
                new[]
                    {
                        new CommunicationSubject("b"), 
                        new CommunicationSubject("c"), 
                    });
            Assert.IsTrue(approver.IsEndpointAllowedToConnect(description));
        }

        [Test]
        public void IsEndpointAllowedToConnectWithNonApprovedEndpoint()
        {
            var subjects = new List<CommunicationSubject>
                {
                    new CommunicationSubject("a"),
                    new CommunicationSubject("b")
                };
            var storage = new Mock<IStoreProtocolSubjects>();
            {
                storage.Setup(s => s.Subjects())
                    .Returns(subjects);
            }

            var approver = new EndpointConnectionApprover(storage.Object);

            var description = new CommunicationDescription(
                new[]
                    {
                        new CommunicationSubject("c"), 
                        new CommunicationSubject("d"), 
                    });
            Assert.IsFalse(approver.IsEndpointAllowedToConnect(description));
        }
    }
}
