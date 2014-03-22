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
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProtocolHandshakeConductorTest
    {
        [Test]
        public void HandshakeWithSuccessResponseWithLocalSendFirst()
        {
            var id = new EndpointId("a:10");
            var subject = new CommunicationSubject("a");
            var connection = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("net.tcp://localhost/protocol/invalid")));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"net.tcp://othermachine";
            var remoteDataAddress = @"net.tcp://othermachine/data";

            var communicationDescriptions = new Mock<IStoreProtocolSubjects>();
            {
                communicationDescriptions.Setup(c => c.Subjects())
                    .Returns(
                        new[]
                            {
                                subject
                            });
                communicationDescriptions.Setup(c => c.ToStorage())
                    .Returns(
                        new ProtocolDescription(
                            new[]
                                {
                                    subject
                                }));
            }

            var endpointApprover = new Mock<IApproveEndpointConnections>();
            {
                endpointApprover.Setup(e => e.ProtocolVersion)
                    .Returns(new Version(1, 0));
                endpointApprover.Setup(e => e.IsEndpointAllowedToConnect(It.IsAny<ProtocolDescription>()))
                    .Returns(true);
            }

            Func<Uri> discoveryUri = () => new Uri("net.tcp://localhost/discovery/invalid");
            var discoveryInformation = new LocalConnectionInformation(id, discoveryUri);
            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.Endpoint);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<IProtocolLayer>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id, 
                            connection.ProtocolInformation.MessageAddress, 
                            connection.ProtocolInformation.DataAddress));
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

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    }, 
                communicationLayer.Object,
                communicationDescriptions.Object,
                new[]
                    {
                        endpointApprover.Object
                    },
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
                        new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")), 
                        new ProtocolInformation(new Version(1, 0), new Uri(remoteMessageAddress)))));

            Assert.IsTrue(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            layer.ContinueHandshakeWith(
                new EndpointInformation(
                    remoteEndpoint,
                    new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")), 
                    new ProtocolInformation(new Version(1, 0), new Uri(remoteMessageAddress), new Uri(remoteDataAddress))),
                communicationDescriptions.Object.ToStorage(),
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
            var connection = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("net.tcp://localhost/protocol/invalid")));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"net.tcp://othermachine";
            var remoteDataAddress = @"net.tcp://othermachine/data";

            var communicationDescriptions = new Mock<IStoreProtocolSubjects>();
            {
                communicationDescriptions.Setup(c => c.Subjects())
                    .Returns(
                        new[]
                            {
                                subject
                            });
                communicationDescriptions.Setup(c => c.ToStorage())
                    .Returns(
                        new ProtocolDescription(
                            new[]
                                {
                                    subject
                                }));
            }

            var endpointApprover = new Mock<IApproveEndpointConnections>();
            {
                endpointApprover.Setup(e => e.ProtocolVersion)
                    .Returns(new Version(1, 0));
                endpointApprover.Setup(e => e.IsEndpointAllowedToConnect(It.IsAny<ProtocolDescription>()))
                    .Returns(true);
            }

            Func<Uri> discoveryUri = () => new Uri("net.tcp://localhost/discovery/invalid");
            var discoveryInformation = new LocalConnectionInformation(id, discoveryUri);
            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.Endpoint);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<IProtocolLayer>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
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

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions.Object,
                new[]
                    {
                        endpointApprover.Object
                    },
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            layer.ContinueHandshakeWith(
                new EndpointInformation(
                    remoteEndpoint,
                    new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")), 
                    new ProtocolInformation(new Version(1, 0), new Uri(remoteMessageAddress), new Uri(remoteDataAddress))),
                communicationDescriptions.Object.ToStorage(),
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
            var connection = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("net.tcp://localhost/protocol/invalid")));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"net.tcp://othermachine";

            var communicationDescriptions = new Mock<IStoreProtocolSubjects>();
            {
                communicationDescriptions.Setup(c => c.Subjects())
                    .Returns(
                        new[]
                            {
                                subject
                            });
                communicationDescriptions.Setup(c => c.ToStorage())
                    .Returns(
                        new ProtocolDescription(
                            new[]
                                {
                                    subject
                                }));
            }

            var endpointApprover = new Mock<IApproveEndpointConnections>();
            {
                endpointApprover.Setup(e => e.ProtocolVersion)
                    .Returns(new Version(1, 0));
                endpointApprover.Setup(e => e.IsEndpointAllowedToConnect(It.IsAny<ProtocolDescription>()))
                    .Returns(true);
            }

            Func<Uri> discoveryUri = () => new Uri("net.tcp://localhost/discovery/invalid");
            var discoveryInformation = new LocalConnectionInformation(id, discoveryUri);
            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.Endpoint);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<IProtocolLayer>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
                communicationLayer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
                communicationLayer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new FailureMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions.Object,
                new[]
                    {
                        endpointApprover.Object
                    },
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
                        new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")), 
                        new ProtocolInformation(
                            new Version(1, 0), 
                            new Uri(remoteMessageAddress)))));

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
            var connection = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("net.tcp://localhost/protocol/invalid")));

            var remoteEndpoint = new EndpointId("b:10");
            var remoteMessageAddress = @"net.tcp://othermachine";
            var remoteDataAddress = @"net.tcp://othermachine/data";

            var communicationDescriptions = new Mock<IStoreProtocolSubjects>();
            {
                communicationDescriptions.Setup(c => c.Subjects())
                    .Returns(
                        new[]
                            {
                                subject
                            });
                communicationDescriptions.Setup(c => c.ToStorage())
                    .Returns(
                        new ProtocolDescription(
                            new[]
                                {
                                    subject
                                }));
            }

            var endpointApprover = new Mock<IApproveEndpointConnections>();
            {
                endpointApprover.Setup(e => e.ProtocolVersion)
                    .Returns(new Version(1, 0));
                endpointApprover.Setup(e => e.IsEndpointAllowedToConnect(It.IsAny<ProtocolDescription>()))
                    .Returns(false);
            }

            Func<Uri> discoveryUri = () => new Uri("net.tcp://localhost/discovery/invalid");
            var discoveryInformation = new LocalConnectionInformation(id, discoveryUri);
            var storage = new EndpointInformationStorage();

            var endpointConnected = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    endpointConnected = true;
                    Assert.AreEqual(remoteEndpoint, e.Endpoint);
                };

            var discovery = new Mock<IDiscoverOtherServices>();
            var communicationLayer = new Mock<IProtocolLayer>();
            {
                communicationLayer.Setup(l => l.Id)
                    .Returns(id);
                communicationLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
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

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                communicationLayer.Object,
                communicationDescriptions.Object,
                new[]
                    {
                        endpointApprover.Object
                    },
                new[]
                    {
                        ChannelTemplate.NamedPipe,
                        ChannelTemplate.TcpIP, 
                    },
                new SystemDiagnostics((l, m) => { }, null));

            layer.ContinueHandshakeWith(
                new EndpointInformation(
                    remoteEndpoint,
                    new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")), 
                    new ProtocolInformation(
                        new Version(1, 0), 
                        new Uri(remoteMessageAddress), 
                        new Uri(remoteDataAddress))), 
                new ProtocolDescription(new List<CommunicationSubject>
                        {
                            new CommunicationSubject("b")
                        }),
                new MessageId());

            Assert.IsFalse(endpointConnected);

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(remoteEndpoint));

            communicationLayer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }
    }
}
