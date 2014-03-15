//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInteractionInformationProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var handshake = new Mock<IHandleInteractionHandshakes>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointInteractionInformationProcessAction(handshake.Object, systemDiagnostics);
            Assert.AreEqual(typeof(EndpointInteractionInformationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var endpoint = new EndpointId("a");
            var group = new[]
                {
                    new CommunicationSubjectGroup(
                        new CommunicationSubject("a"),
                        new[]
                            {
                                new VersionedTypeFallback(
                                    new Tuple<OfflineTypeInformation, Version>(
                                        new OfflineTypeInformation(
                                            typeof(int).FullName,
                                            typeof(int).Assembly.GetName()),
                                        new Version(1, 0))),
                            },
                        new[]
                            {
                                new VersionedTypeFallback(
                                    new Tuple<OfflineTypeInformation, Version>(
                                        new OfflineTypeInformation(
                                            typeof(double).FullName,
                                            typeof(double).Assembly.GetName()),
                                        new Version(1, 2))),
                            }),
                };
            var message = new EndpointInteractionInformationMessage(endpoint, group);

            var handshake = new Mock<IHandleInteractionHandshakes>();
            {
                handshake.Setup(h => h.ContinueHandshakeWith(It.IsAny<EndpointId>(), It.IsAny<CommunicationSubjectGroup[]>(), It.IsAny<MessageId>()))
                    .Callback<EndpointId, CommunicationSubjectGroup[], MessageId>(
                        (e, c, m) =>
                        {
                            Assert.AreSame(endpoint, e);
                            Assert.AreSame(group, c);
                            Assert.AreSame(message.Id, m);
                        })
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointInteractionInformationProcessAction(handshake.Object, systemDiagnostics);
            action.Invoke(message);

            handshake.Verify(
                h => h.ContinueHandshakeWith(It.IsAny<EndpointId>(), It.IsAny<CommunicationSubjectGroup[]>(), It.IsAny<MessageId>()),
                Times.Once());
        }
    }
}
