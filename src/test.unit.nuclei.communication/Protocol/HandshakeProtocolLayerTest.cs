//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class HandshakeProtocolLayerTest
    {
        [Test]
        public void HandshakeWithSuccessResponseWithLocalSendFirst()
        {
            var id = new EndpointId("a:10");
            var subject = new CommunicationSubject("a");
            var connection = new ChannelConnectionInformation(
                id,
                ChannelTemplate.TcpIP,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"http://othermachine";
            var remoteDataAddress = @"http://othermachine/data";

            var communicationDescriptions = new CommunicationDescriptionStorage();
            communicationDescriptions.RegisterApplicationSubject(subject);

            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.ConnectionInformation.Id);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<ISendDataViaChannels>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<ChannelTemplate>()))
                    .Returns(new Tuple<EndpointId, Uri, Uri>(connection.Id, connection.MessageAddress, connection.DataAddress));
                communicationLayer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
                communicationLayer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None, 
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var layer = new HandshakeConductor(
                storage,
                new[]
                    {
                        discovery.Object
                    }, 
                communicationLayer.Object,
                communicationDescriptions,
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            discovery.Raise(
                d => d.OnEndpointBecomingAvailable += null, 
                new EndpointDiscoveredEventArgs(
                    new EndpointInformation(
                        remoteEndpoint, 
                        new Version(1, 0), 
                        new Uri(remoteMessageAddress))));

            Assert.IsTrue(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            layer.ContinueHandshakeWith(
                new ChannelConnectionInformation(remoteEndpoint, ChannelTemplate.TcpIP, new Uri(remoteMessageAddress), new Uri(remoteDataAddress)),
                communicationDescriptions.ToStorage(),
                new MessageId());

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            Assert.IsTrue(endpointConnected);
            
            communicationLayer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            communicationLayer.Verify(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void HandshakeWithSuccessResponseWithRemoteSendFirst()
        {
            var id = new EndpointId("a:10");
            var subject = new CommunicationSubject("a");
            var connection = new ChannelConnectionInformation(
                id,
                ChannelTemplate.TcpIP,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"http://othermachine"; 
            var remoteDataAddress = @"http://othermachine/data";

            var communicationDescriptions = new CommunicationDescriptionStorage();
            communicationDescriptions.RegisterApplicationSubject(subject);

            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.ConnectionInformation.Id);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<ISendDataViaChannels>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<ChannelTemplate>()))
                    .Returns(new Tuple<EndpointId, Uri, Uri>(connection.Id, connection.MessageAddress, connection.DataAddress));
                communicationLayer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
                communicationLayer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var layer = new HandshakeConductor(
                storage,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions,
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            layer.ContinueHandshakeWith(
                new ChannelConnectionInformation(remoteEndpoint, ChannelTemplate.TcpIP, new Uri(remoteMessageAddress), new Uri(remoteDataAddress)),
                communicationDescriptions.ToStorage(),
                new MessageId());

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            Assert.IsTrue(endpointConnected);

            communicationLayer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            communicationLayer.Verify(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void HandshakeWithWithRemoteRejection()
        {
            var id = new EndpointId("a:10");
            var subject = new CommunicationSubject("a");
            var connection = new ChannelConnectionInformation(
                id,
                ChannelTemplate.TcpIP,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"http://othermachine";

            var communicationDescriptions = new CommunicationDescriptionStorage();
            communicationDescriptions.RegisterApplicationSubject(subject);

            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.ConnectionInformation.Id);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<ISendDataViaChannels>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<ChannelTemplate>()))
                    .Returns(new Tuple<EndpointId, Uri, Uri>(connection.Id, connection.MessageAddress, connection.DataAddress));
                communicationLayer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
                communicationLayer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                        })
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new FailureMessage(remoteEndpoint, MessageId.None),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var layer = new HandshakeConductor(
                storage,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions,
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            Assert.IsNotNull(layer);
            discovery.Raise(
                d => d.OnEndpointBecomingAvailable += null,
                new EndpointDiscoveredEventArgs(
                    new EndpointInformation(
                        remoteEndpoint,
                        new Version(1, 0), 
                        new Uri(remoteMessageAddress))));

            Assert.IsFalse(endpointConnected);

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            communicationLayer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
            communicationLayer.Verify(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void HandshakeWithLocalRejection()
        {
            var id = new EndpointId("a:10");
            var subject = new CommunicationSubject("a");
            var connection = new ChannelConnectionInformation(
                id,
                ChannelTemplate.TcpIP,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"http://othermachine";
            var remoteDataAddress = @"http://othermachine/data";

            var communicationDescriptions = new CommunicationDescriptionStorage();
            communicationDescriptions.RegisterApplicationSubject(subject);

            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.ConnectionInformation.Id);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<ISendDataViaChannels>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<ChannelTemplate>()))
                    .Returns(new Tuple<EndpointId, Uri, Uri>(connection.Id, connection.MessageAddress, connection.DataAddress));
                communicationLayer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            var layer = new HandshakeConductor(
                storage,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions,
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            layer.ContinueHandshakeWith(
                new ChannelConnectionInformation(remoteEndpoint, ChannelTemplate.TcpIP, new Uri(remoteMessageAddress), new Uri(remoteDataAddress)), 
                new CommunicationDescription(new List<CommunicationSubject>
                        {
                            new CommunicationSubject("b")
                        },
                    new List<ISerializedType>(),
                    new List<ISerializedType>()),
                new MessageId());

            Assert.IsFalse(endpointConnected);

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            communicationLayer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }
    }
}
