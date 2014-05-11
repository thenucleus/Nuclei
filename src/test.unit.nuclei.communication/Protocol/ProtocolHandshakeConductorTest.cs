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
using Nuclei.Configuration;
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

            Func<ChannelTemplate, Uri> discoveryUri = t => new Uri("net.tcp://localhost/discovery/invalid");
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
            var protocolLayer = new Mock<IProtocolLayer>();
            {
                protocolLayer.Setup(l => l.Id)
                    .Returns(id);
                protocolLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id, 
                            connection.ProtocolInformation.MessageAddress, 
                            connection.ProtocolInformation.DataAddress));
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpoint(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ICommunicationMessage>(), 
                        It.IsAny<int>()))
                    .Verifiable();
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ICommunicationMessage>(),
                        It.IsAny<int>(),
                        It.IsAny<TimeSpan>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None, 
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    }, 
                protocolLayer.Object,
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
                configuration.Object,
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

            var remoteInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(new Version(1, 0), new Uri(remoteMessageAddress), new Uri(remoteDataAddress)));
            layer.ContinueHandshakeWith(remoteInfo, communicationDescriptions.Object.ToStorage(), new MessageId());

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(remoteEndpoint));
            Assert.IsTrue(endpointConnected);

            EndpointInformation info;
            var tryGet = storage.TryGetConnectionFor(remoteEndpoint, out info);
            Assert.IsTrue(tryGet);
            Assert.AreEqual(remoteInfo.DiscoveryInformation.Address, info.DiscoveryInformation.Address);
            Assert.AreEqual(remoteInfo.ProtocolInformation.Version, info.ProtocolInformation.Version);
            Assert.AreEqual(remoteInfo.ProtocolInformation.MessageAddress, info.ProtocolInformation.MessageAddress);
            Assert.AreEqual(remoteInfo.ProtocolInformation.DataAddress, info.ProtocolInformation.DataAddress);

            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpoint(It.IsAny<EndpointInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), 
                Times.Once());
            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                    It.IsAny<EndpointInformation>(), 
                    It.IsAny<ICommunicationMessage>(),
                    It.IsAny<int>(),
                    It.IsAny<TimeSpan>()), 
                Times.Once());
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

            Func<ChannelTemplate, Uri> discoveryUri = t => new Uri("net.tcp://localhost/discovery/invalid");
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
            var protocolLayer = new Mock<IProtocolLayer>();
            {
                protocolLayer.Setup(l => l.Id)
                    .Returns(id);
                protocolLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpoint(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ICommunicationMessage>(), 
                        It.IsAny<int>()))
                    .Verifiable();
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                        It.IsAny<EndpointInformation>(),
                        It.IsAny<ICommunicationMessage>(),
                        It.IsAny<int>(),
                        It.IsAny<TimeSpan>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                protocolLayer.Object,
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
                configuration.Object,
                new SystemDiagnostics((l, m) => { }, null));

            var remoteInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.tcp://localhost/discovery/invalid")),
                new ProtocolInformation(new Version(1, 0), new Uri(remoteMessageAddress), new Uri(remoteDataAddress)));
            layer.ContinueHandshakeWith(remoteInfo, communicationDescriptions.Object.ToStorage(), new MessageId());

            Assert.IsFalse(storage.HasBeenContacted(remoteEndpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(remoteEndpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(remoteEndpoint));
            Assert.IsTrue(endpointConnected);

            EndpointInformation info;
            var tryGet = storage.TryGetConnectionFor(remoteEndpoint, out info);
            Assert.IsTrue(tryGet);
            Assert.AreEqual(remoteInfo.DiscoveryInformation.Address, info.DiscoveryInformation.Address);
            Assert.AreEqual(remoteInfo.ProtocolInformation.Version, info.ProtocolInformation.Version);
            Assert.AreEqual(remoteInfo.ProtocolInformation.MessageAddress, info.ProtocolInformation.MessageAddress);
            Assert.AreEqual(remoteInfo.ProtocolInformation.DataAddress, info.ProtocolInformation.DataAddress);

            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpoint(It.IsAny<EndpointInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), 
                Times.Once());
            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                    It.IsAny<EndpointInformation>(), 
                    It.IsAny<ICommunicationMessage>(),
                    It.IsAny<int>(),
                    It.IsAny<TimeSpan>()),
                Times.Once());
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

            Func<ChannelTemplate, Uri> discoveryUri = t => new Uri("net.tcp://localhost/discovery/invalid");
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
            var protocolLayer = new Mock<IProtocolLayer>();
            {
                protocolLayer.Setup(l => l.Id)
                    .Returns(id);
                protocolLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpoint(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ICommunicationMessage>(), 
                        It.IsAny<int>()))
                    .Verifiable();
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                        It.IsAny<EndpointInformation>(),
                        It.IsAny<ICommunicationMessage>(),
                        It.IsAny<int>(),
                        It.IsAny<TimeSpan>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new FailureMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                protocolLayer.Object,
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
                configuration.Object,
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

            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpoint(It.IsAny<EndpointInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), 
                Times.Never());
            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                    It.IsAny<EndpointInformation>(), 
                    It.IsAny<ICommunicationMessage>(),
                    It.IsAny<int>(),
                    It.IsAny<TimeSpan>()),
                Times.Once());
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

            Func<ChannelTemplate, Uri> discoveryUri = t => new Uri("net.tcp://localhost/discovery/invalid");
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
            var protocolLayer = new Mock<IProtocolLayer>();
            {
                protocolLayer.Setup(l => l.Id)
                    .Returns(id);
                protocolLayer.Setup(l => l.LocalConnectionFor(It.IsAny<Version>(), It.IsAny<ChannelTemplate>()))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            connection.Id,
                            connection.ProtocolInformation.MessageAddress,
                            connection.ProtocolInformation.DataAddress));
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpoint(
                        It.IsAny<EndpointInformation>(), 
                        It.IsAny<ICommunicationMessage>(), 
                        It.IsAny<int>()))
                    .Verifiable();
                protocolLayer.Setup(l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                        It.IsAny<EndpointInformation>(),
                        It.IsAny<ICommunicationMessage>(),
                        It.IsAny<int>(),
                        It.IsAny<TimeSpan>()))
                    .Returns(Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var layer = new ProtocolHandshakeConductor(
                storage,
                discoveryInformation,
                new[]
                    {
                        discovery.Object
                    },
                protocolLayer.Object,
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
                configuration.Object,
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

            protocolLayer.Verify(
                l => l.SendMessageToUnregisteredEndpointAndWaitForResponse(
                    It.IsAny<EndpointInformation>(), 
                    It.IsAny<ICommunicationMessage>(),
                    It.IsAny<int>(),
                    It.IsAny<TimeSpan>()),
                Times.Never());
        }
    }
}
