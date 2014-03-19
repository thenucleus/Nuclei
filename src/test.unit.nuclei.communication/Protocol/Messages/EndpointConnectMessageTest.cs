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
    }
}
