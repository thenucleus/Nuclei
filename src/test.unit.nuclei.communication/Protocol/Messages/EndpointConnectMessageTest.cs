//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            var sender = new EndpointId("sendingEndpoint");
            var discovery = new DiscoveryInformation(new Uri("http://localhost/discovery/invalid"));
            var protocol = new ProtocolInformation(
                new Version(1, 0),
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var description = new ProtocolDescription(new List<CommunicationSubject>());
            var msg = new EndpointConnectMessage(
                sender, 
                discovery, 
                protocol, 
                description);

            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(discovery, msg.DiscoveryInformation);
            Assert.AreSame(protocol, msg.ProtocolInformation);
            Assert.AreSame(description, msg.Information);
        }

        [Test]
        public void CreateWithId()
        {
            var sender = new EndpointId("sendingEndpoint");
            var id = new MessageId();
            var discovery = new DiscoveryInformation(new Uri("http://localhost/discovery/invalid"));
            var protocol = new ProtocolInformation(
                new Version(1, 0),
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var description = new ProtocolDescription(new List<CommunicationSubject>());
            var msg = new EndpointConnectMessage(
                sender,
                id,
                discovery,
                protocol,
                description);

            Assert.AreSame(id, msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(discovery, msg.DiscoveryInformation);
            Assert.AreSame(protocol, msg.ProtocolInformation);
            Assert.AreSame(description, msg.Information);
        }
    }
}
