//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProtocolLayerTest
    {
        [Test]
        public void OnEndpointApproved()
        {
            var endpoint = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder = 
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(null, null);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var eventWasRaised = false;
            layer.OnEndpointConnected +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(endpoint, e.Endpoint);
                };

            endpoints.Raise(d => d.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        public void OnEndpointDisconnected()
        {
            var endpoint = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(null, null);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var connectedEventWasRaised = false;
            layer.OnEndpointConnected +=
                (s, e) =>
                {
                    connectedEventWasRaised = true;
                    Assert.AreEqual(endpoint, e.Endpoint);
                };

            var disconnectedEventWasRaised = false;
            layer.OnEndpointDisconnected +=
                (s, e) =>
                {
                    disconnectedEventWasRaised = true;
                    Assert.AreEqual(endpoint, e.Endpoint);
                };

            endpoints.Raise(d => d.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsTrue(connectedEventWasRaised);
            Assert.IsFalse(disconnectedEventWasRaised);

            connectedEventWasRaised = false;
            endpoints.Raise(d => d.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsFalse(connectedEventWasRaised);
            Assert.IsTrue(disconnectedEventWasRaised);
        }

        [Test]
        public void SignIn()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var protocolInfo = new ProtocolInformation(
                new Version(1, 0),
                new Uri("net.pipe://localhost/protocol/message/invalid"),
                new Uri("net.pipe://localhost/protocol/data/invalid"));
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(protocolInfo);
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();
            Assert.IsTrue(layer.IsSignedIn);

            var connection = layer.LocalConnectionFor(new Version(1, 0), ChannelTemplate.NamedPipe);
            Assert.AreSame(protocolInfo.MessageAddress, connection.Item2);
            Assert.AreSame(protocolInfo.DataAddress, connection.Item3);

            channel.Verify(c => c.OpenChannel(), Times.Once());
        }

        [Test]
        public void OnEndpointDisconnectedAfterSigningIn()
        {
            var endpoint = new EndpointId("a");
            var endpointInfo = new EndpointInformation(
                endpoint, 
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.EndpointDisconnected(It.IsAny<ProtocolInformation>()))
                    .Callback<ProtocolInformation>(e => Assert.AreEqual(endpointInfo.ProtocolInformation, e))
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.OnEndpointDisconnected(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreEqual(endpoint, e))
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();
            endpoints.Raise(d => d.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));

            channel.Verify(c => c.EndpointDisconnected(It.IsAny<ProtocolInformation>()), Times.Once());
            pipe.Verify(p => p.OnEndpointDisconnected(It.IsAny<EndpointId>()), Times.Once());
        }

        [Test]
        public void SignInWhileSignedIn()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            
            var protocolInfo = new ProtocolInformation(
                new Version(1, 0),
                new Uri("net.pipe://localhost/protocol/message/invalid"),
                new Uri("net.pipe://localhost/protocol/data/invalid"));
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(protocolInfo);
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();
            Assert.IsTrue(layer.IsSignedIn);
            channel.Verify(c => c.OpenChannel(), Times.Once());

            layer.SignIn();
            Assert.IsTrue(layer.IsSignedIn);
            channel.Verify(c => c.OpenChannel(), Times.Once());
        }

        [Test]
        public void SignOut()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            
            var protocolInfo = new ProtocolInformation(
                new Version(1, 0),
                new Uri("net.pipe://localhost/protocol/message/invalid"),
                new Uri("net.pipe://localhost/protocol/data/invalid"));
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(protocolInfo);
                channel.Setup(c => c.CloseChannel())
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();
            Assert.IsTrue(layer.IsSignedIn);
            channel.Verify(c => c.OpenChannel(), Times.Once());

            layer.SignOut();
            Assert.IsFalse(layer.IsSignedIn);
            channel.Verify(c => c.CloseChannel(), Times.Once());
        }

        [Test]
        public void SignOutWithoutBeingSignedIn()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            
            var protocolInfo = new ProtocolInformation(
                new Version(1, 0),
                new Uri("net.pipe://localhost/protocol/message/invalid"),
                new Uri("net.pipe://localhost/protocol/data/invalid"));
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(protocolInfo);
                channel.Setup(c => c.CloseChannel())
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignOut();
            Assert.IsFalse(layer.IsSignedIn);
            channel.Verify(c => c.CloseChannel(), Times.Never());
        }

        [Test]
        public void IsEndpointContactableWithNonContactableEndpoint()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var channel = new Mock<IProtocolChannel>();
            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var endpoint = new EndpointId("a");
            Assert.IsFalse(layer.IsEndpointContactable(endpoint));
        }

        [Test]
        public void IsEndpointContactableWithContactableEndpoint()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
            }

            var channel = new Mock<IProtocolChannel>();
            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var endpoint = new EndpointId("a");
            Assert.IsTrue(layer.IsEndpointContactable(endpoint));
        }

        [Test]
        public void SendMessageToWithUncontactableEndpoint()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var channel = new Mock<IProtocolChannel>();
            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            Assert.Throws<EndpointNotContactableException>(
                () => layer.SendMessageTo(new EndpointId("A"), new SuccessMessage(new EndpointId("B"), new MessageId())));
        }

        [Test]
        public void SendMessageToWithUnopenedChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<ProtocolInformation, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SendMessageTo(remoteEndpoint, msg);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void SendMessageToWithOpenChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<ProtocolInformation, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();

            layer.SendMessageTo(remoteEndpoint, msg);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithUncontactableEndpoint()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var channel = new Mock<IProtocolChannel>();
            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            Assert.Throws<EndpointNotContactableException>(
                () => layer.SendMessageAndWaitForResponse(new EndpointId("A"), new SuccessMessage(new EndpointId("B"), new MessageId())));
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithUnopenedChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<ProtocolInformation, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => new SuccessMessage(remoteEndpoint, msg.Id),
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>()))
                    .Callback<EndpointId, MessageId>(
                        (e, m) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreSame(msg.Id, m);
                        })
                    .Returns(responseTask)
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var response = layer.SendMessageAndWaitForResponse(remoteEndpoint, msg);
            Assert.AreSame(responseTask, response);
            
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>()), Times.Once());
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithOpenedChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<ProtocolInformation, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => new SuccessMessage(remoteEndpoint, msg.Id),
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>()))
                    .Callback<EndpointId, MessageId>(
                        (e, m) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreSame(msg.Id, m);
                        })
                    .Returns(responseTask)
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();

            var response = layer.SendMessageAndWaitForResponse(remoteEndpoint, msg);
            Assert.AreSame(responseTask, response);

            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>()), Times.Once());
        }

        [Test]
        public void UploadDataWithUncontactableEndpoint()
        {
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var protocolInfo = new ProtocolInformation(
                new Version(1, 0),
                new Uri("net.pipe://localhost/protocol/message/invalid"),
                new Uri("net.pipe://localhost/protocol/data/invalid"));
            var endpoint = new EndpointId("A");
            var file = "B";
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(protocolInfo);
                channel.Setup(
                    c => c.TransferData(
                        It.IsAny<ProtocolInformation>(), 
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>(), 
                        It.IsAny<TaskScheduler>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) =>
                        {
                            Assert.AreSame(protocolInfo, e);
                            Assert.AreSame(file, p);
                        })
                    .Returns<EndpointId, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) => Task.Factory.StartNew(() => { }, c, TaskCreationOptions.None, s))
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();

            Assert.Throws<EndpointNotContactableException>(
                () => layer.UploadData(endpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler()));
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(It.IsAny<ProtocolInformation>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<TaskScheduler>()),
                Times.Never());
        }

        [Test]
        public void UploadDataWithUnopenedChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var file = "B";
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(
                    c => c.TransferData(
                        It.IsAny<ProtocolInformation>(), 
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>(), 
                        It.IsAny<TaskScheduler>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(file, p);
                        })
                    .Returns<ProtocolInformation, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) => Task.Factory.StartNew(() => { }, c, TaskCreationOptions.None, s))
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();

            layer.UploadData(remoteEndpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler());
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(It.IsAny<ProtocolInformation>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<TaskScheduler>()),
                Times.Once());
        }

        [Test]
        public void UploadDataWithOpenedChannel()
        {
            var remoteEndpoint = new EndpointId("b");
            var endpointInfo = new EndpointInformation(
                remoteEndpoint,
                new DiscoveryInformation(new Uri("net.pipe://localhost/discovery")),
                new ProtocolInformation(
                    ProtocolVersions.V1,
                    new Uri("net.pipe://localhost/messages"),
                    new Uri("net.pipe://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true);
            }

            var file = "B";
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(
                    c => c.TransferData(
                        It.IsAny<ProtocolInformation>(), 
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>(), 
                        It.IsAny<TaskScheduler>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(file, p);
                        })
                    .Returns<ProtocolInformation, string, CancellationToken, TaskScheduler>(
                        (e, p, c, s) => Task.Factory.StartNew(() => { }, c, TaskCreationOptions.None, s))
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) =>
                {
                    return new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
                };
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SignIn();

            layer.UploadData(remoteEndpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler());
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(It.IsAny<ProtocolInformation>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<TaskScheduler>()), 
                Times.Once());
        }
    }
}
