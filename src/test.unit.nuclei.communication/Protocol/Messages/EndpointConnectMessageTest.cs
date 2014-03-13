//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var discovery = new DiscoveryInformation(new Uri("http://localhost/discovery/invalid"));
            var protocol = new ProtocolInformation(
                new Version(1, 0),
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var description = new ProtocolDescription(new List<CommunicationSubject>());
            var msg = new EndpointConnectMessage(
                id, 
                discovery, 
                protocol, 
                description);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(discovery, msg.DiscoveryInformation);
            Assert.AreEqual(protocol, msg.ProtocolInformation);
            Assert.AreSame(description, msg.Information);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var discovery = new DiscoveryInformation(new Uri("http://localhost/discovery/invalid"));
            var protocol = new ProtocolInformation(
                new Version(1, 0),
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var description = new ProtocolDescription(
                new List<CommunicationSubject>
                    {
                        new CommunicationSubject("a")
                    });
            var msg = new EndpointConnectMessage(
                id,
                discovery,
                protocol,
                description);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.Sender);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.AreEqual(discovery.Address, otherMsg.DiscoveryInformation.Address);
            Assert.AreEqual(protocol.Version, otherMsg.DiscoveryInformation.Address);
            Assert.AreEqual(protocol.MessageAddress, otherMsg.ProtocolInformation.MessageAddress);
            Assert.AreEqual(protocol.DataAddress, otherMsg.ProtocolInformation.DataAddress);
            Assert.That(otherMsg.Information.Subjects, Is.EquivalentTo(description.Subjects));
        }
    }
}
