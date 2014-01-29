//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.Messages.Processors;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MessageHandlerTest
    {
        [Test]
        public void ForwardResponse()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new MessageHandler(store.Object, systemDiagnostics);

            var endpoint = new EndpointId("sendingEndpoint");
            var messageId = new MessageId();
            var task = handler.ForwardResponse(endpoint, messageId);
            Assert.IsFalse(task.IsCompleted);

            var msg = new SuccessMessage(endpoint, messageId);
            handler.ProcessMessage(msg);

            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreSame(msg, task.Result);
        }

        [Test]
        public void ForwardResponseWithDisconnectingEndpoint()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new MessageHandler(store.Object, systemDiagnostics);

            var endpoint = new EndpointId("sendingEndpoint");
            var messageId = new MessageId();
            var task = handler.ForwardResponse(endpoint, messageId);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            var msg = new EndpointDisconnectMessage(endpoint);
            handler.ProcessMessage(msg);

            Assert.Throws<AggregateException>(task.Wait);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public void ActOnArrivalWithMessageFromNonBlockedSender()
        {
            ICommunicationMessage storedMessage = null;
            var processAction = new Mock<IMessageProcessAction>();
            {
                processAction.Setup(p => p.Invoke(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(m => { storedMessage = m; });
            }

            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new MessageHandler(store.Object, systemDiagnostics);

            handler.ActOnArrival(new MessageKindFilter(typeof(SuccessMessage)), processAction.Object);

            var endpoint = new EndpointId("sendingEndpoint");
            var msg = new SuccessMessage(endpoint, MessageId.None);
            handler.ProcessMessage(msg);

            Assert.AreSame(msg, storedMessage);
        }

        [Test]
        public void ActOnArrivalWithMessageFromBlockedSender()
        {
            ICommunicationMessage storedMessage = null;
            var processAction = new Mock<IMessageProcessAction>();
            {
                processAction.Setup(p => p.Invoke(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(m => { storedMessage = m; });
            }

            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new MessageHandler(store.Object, systemDiagnostics);

            handler.ActOnArrival(new MessageKindFilter(typeof(SuccessMessage)), processAction.Object);

            var endpoint = new EndpointId("sendingEndpoint");
            var msg = new SuccessMessage(endpoint, new MessageId());
            handler.ProcessMessage(msg);

            Assert.IsNull(storedMessage);
        }

        [Test]
        public void ActOnArrivalWithHandshakeMessage()
        {
            ICommunicationMessage storedMessage = null;
            var processAction = new Mock<IMessageProcessAction>();
            {
                processAction.Setup(p => p.Invoke(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(m => { storedMessage = m; });
            }

            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new MessageHandler(store.Object, systemDiagnostics);

            handler.ActOnArrival(new MessageKindFilter(typeof(EndpointConnectMessage)), processAction.Object);

            var endpoint = new EndpointId("sendingEndpoint");
            var msg = new EndpointConnectMessage(
                endpoint, 
                ChannelTemplate.NamedPipe,
                @"net.pipe://localhost/test", 
                @"net.pipe://localhost/test/data",
                new CommunicationDescription(new List<CommunicationSubject>(), 
                    new List<ISerializedType>(), 
                    new List<ISerializedType>()));
            handler.ProcessMessage(msg);

            Assert.AreSame(msg, storedMessage);
        }

        [Test]
        public void ActOnArrivalWithLastChanceHandler()
        {
            var localEndpoint = new EndpointId("id");

            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
            {
                storedMsg = m;
            };

            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var processAction = new UnknownMessageTypeProcessAction(localEndpoint, sendAction, systemDiagnostics);
            var handler = new MessageHandler(store.Object, systemDiagnostics);
            handler.ActOnArrival(new MessageKindFilter(processAction.MessageTypeToProcess), processAction);

            var endpoint = new EndpointId("sendingEndpoint");
            var msg = new EndpointConnectMessage(
                endpoint,
                ChannelTemplate.NamedPipe,
                @"net.pipe://localhost/test",
                @"net.pipe://localhost/test/data", 
                new CommunicationDescription(new List<CommunicationSubject>(),
                    new List<ISerializedType>(),
                    new List<ISerializedType>()));
            handler.ProcessMessage(msg);

            Assert.IsInstanceOf<UnknownMessageTypeMessage>(storedMsg);
        }
    }
}
