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

namespace Nuclei.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<IHandleHandshakes>();
            var channelTypes = new[] 
                { 
                    ChannelType.TcpIP
                };
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, systemDiagnostics);
            Assert.AreEqual(typeof(EndpointConnectMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            ChannelConnectionInformation processedChannel = null;
            CommunicationDescription processedDescription = null;
            MessageId processedMessageId = null;
            var sink = new Mock<IHandleHandshakes>();
            {
                sink.Setup(s => s.ContinueHandshakeWith(
                        It.IsAny<ChannelConnectionInformation>(), 
                        It.IsAny<CommunicationDescription>(), 
                        It.IsAny<MessageId>()))
                    .Callback<ChannelConnectionInformation, CommunicationDescription, MessageId>((e, t, u) => 
                        {
                            processedChannel = e;
                            processedDescription = t;
                            processedMessageId = u;
                        });
            }

            var channelTypes = new[] 
                { 
                    ChannelType.TcpIP
                };
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, systemDiagnostics);
            
            var id = new EndpointId("id");
            var type = ChannelType.TcpIP;
            var messageUri = @"http://localhost";
            var dataUri = @"http://localhost/data";
            var description = new CommunicationDescription(
                new Version(1, 0), 
                new List<CommunicationSubject>(), 
                new List<ISerializedType>(), 
                new List<ISerializedType>());
            var msg = new EndpointConnectMessage(id, type, messageUri, dataUri, description);
            action.Invoke(msg);

            Assert.AreEqual(msg.Id, processedMessageId);
            Assert.AreEqual(id, processedChannel.Id);
            Assert.AreEqual(type, processedChannel.ChannelType);
            Assert.AreEqual(messageUri, processedChannel.MessageAddress.OriginalString);
            Assert.AreSame(description, processedDescription);
        }
    }
}
