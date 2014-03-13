//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<IHandleProtocolHandshakes>();
            var channelTypes = new[] 
                { 
                    ChannelTemplate.TcpIP
                };
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, systemDiagnostics);
            Assert.AreEqual(typeof(EndpointConnectMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointInformation processedChannel = null;
            ProtocolDescription processedDescription = null;
            MessageId processedMessageId = null;
            var sink = new Mock<IHandleProtocolHandshakes>();
            {
                sink.Setup(s => s.ContinueHandshakeWith(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ProtocolDescription>(), 
                        It.IsAny<MessageId>()))
                    .Callback<EndpointInformation, ProtocolDescription, MessageId>((e, t, u) => 
                        {
                            processedChannel = e;
                            processedDescription = t;
                            processedMessageId = u;
                        });
            }

            var channelTypes = new[] 
                { 
                    ChannelTemplate.TcpIP
                };
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, systemDiagnostics);
            
            var id = new EndpointId("id");
            var discovery = new DiscoveryInformation(new Uri("http://localhost/discovery/invalid"));
            var protocol = new ProtocolInformation(
                new Version(1, 0), 
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var description = new ProtocolDescription(new List<CommunicationSubject>());
            var msg = new EndpointConnectMessage(id, discovery, protocol, description);
            action.Invoke(msg);

            Assert.AreEqual(msg.Id, processedMessageId);
            Assert.AreEqual(id, processedChannel.Id);
            Assert.AreSame(discovery, processedChannel.DiscoveryInformation);
            Assert.AreSame(protocol, processedChannel.ProtocolInformation);
            Assert.AreSame(description, processedDescription);
        }
    }
}
