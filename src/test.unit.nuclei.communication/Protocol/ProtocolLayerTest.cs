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
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
                pipe.Setup(p => p.OnEndpointSignedOff(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreEqual(endpoint, e))
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
            endpoints.Raise(d => d.OnEndpointDisconnecting += null, new EndpointEventArgs(endpoint));

            channel.Verify(c => c.EndpointDisconnected(It.IsAny<ProtocolInformation>()), Times.Once());
            pipe.Verify(p => p.OnEndpointSignedOff(It.IsAny<EndpointId>()), Times.Once());
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
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
        public void VerifyConnectionIsActiveWithUnknownEndpoint()
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
                () => layer.VerifyConnectionIsActive(
                    new EndpointId("A"),
                    TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void VerifyConnectionIsActiveWithInactiveEndpoint()
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

            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.IsInstanceOf(typeof(ConnectionVerificationMessage), m);
                        })
                    .Verifiable();
            }

            var timeout = TimeSpan.FromSeconds(1);
            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () =>
                {
                    throw new TimeoutException();
                },
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()))
                    .Callback<EndpointId, MessageId, TimeSpan>(
                        (e, m, t) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreEqual(timeout, t);
                        })
                    .Returns(responseTask)
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var response = layer.VerifyConnectionIsActive(remoteEndpoint, timeout);
            Assert.IsTrue(response.IsFaulted);
            Assert.IsNotNull(response.Exception);

            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()), Times.Once());
        }

        [Test]
        public void VerifyConnectionIsActiveWithUnresponsiveEndpoint()
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

            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.IsInstanceOf(typeof(ConnectionVerificationMessage), m);
                        })
                    .Verifiable();
            }

            var timeout = TimeSpan.FromSeconds(1);
            var token = new CancellationTokenSource();
            token.Cancel();
            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => null,
                token.Token,
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()))
                    .Callback<EndpointId, MessageId, TimeSpan>(
                        (e, m, t) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreEqual(timeout, t);
                        })
                    .Returns(responseTask)
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var response = layer.VerifyConnectionIsActive(remoteEndpoint, timeout);
            Assert.IsTrue(response.IsCanceled);

            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()), Times.Once());
        }

        [Test]
        public void VerifyConnectionIsActiveWithActiveEndpoint()
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

            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.IsInstanceOf(typeof(ConnectionVerificationMessage), m);
                        })
                    .Verifiable();
            }

            var responseData = new object();
            var timeout = TimeSpan.FromSeconds(1);
            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => new ConnectionVerificationResponseMessage(remoteEndpoint, new MessageId(), responseData),
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()))
                    .Callback<EndpointId, MessageId, TimeSpan>(
                        (e, m, t) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreEqual(timeout, t);
                        })
                    .Returns(responseTask)
                    .Verifiable();
            }

            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            var response = layer.VerifyConnectionIsActive(remoteEndpoint, timeout);
            Assert.AreSame(responseData, response.Result);

            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()), Times.Once());
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
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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
                () => layer.SendMessageTo(new EndpointId("A"), new SuccessMessage(new EndpointId("B"), new MessageId()), 1));
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
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
            var diagnostics = new SystemDiagnostics((log, s) => { }, null);
            var layer = new ProtocolLayer(
                endpoints.Object,
                channelBuilder,
                new[]
                    {
                        ChannelTemplate.NamedPipe, 
                    },
                diagnostics);

            layer.SendMessageTo(remoteEndpoint, msg, 1);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
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
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                        })
                    .Verifiable();
            }

            var pipe = new Mock<IDirectIncomingMessages>();
            Func<ChannelTemplate, EndpointId, Tuple<IProtocolChannel, IDirectIncomingMessages>> channelBuilder =
                (template, id) => new Tuple<IProtocolChannel, IDirectIncomingMessages>(channel.Object, pipe.Object);
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

            layer.SendMessageTo(remoteEndpoint, msg, 1);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
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
                () => layer.SendMessageAndWaitForResponse(
                    new EndpointId("A"), 
                    new SuccessMessage(new EndpointId("B"), new MessageId()), 
                    1,
                    TimeSpan.FromSeconds(1)));
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

            var retryCount = 1;
            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                            Assert.AreEqual(retryCount, r);
                        })
                    .Verifiable();
            }

            var timeout = TimeSpan.FromSeconds(1);
            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => new SuccessMessage(remoteEndpoint, msg.Id),
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()))
                    .Callback<EndpointId, MessageId, TimeSpan>(
                        (e, m, t) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreSame(msg.Id, m);
                            Assert.AreEqual(timeout, t);
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

            var response = layer.SendMessageAndWaitForResponse(remoteEndpoint, msg, retryCount, timeout);
            Assert.AreSame(responseTask, response);
            
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()), Times.Once());
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

            var retryCount = 1;
            var msg = new SuccessMessage(new EndpointId("B"), new MessageId());
            var channel = new Mock<IProtocolChannel>();
            {
                channel.Setup(c => c.OpenChannel())
                    .Verifiable();
                channel.Setup(c => c.LocalConnectionPointForVersion(It.IsAny<Version>()))
                    .Returns(endpointInfo.ProtocolInformation);
                channel.Setup(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(msg, m);
                            Assert.AreEqual(retryCount, r);
                        })
                    .Verifiable();
            }

            var timeout = TimeSpan.FromSeconds(1);
            var responseTask = Task<ICommunicationMessage>.Factory.StartNew(
                () => new SuccessMessage(remoteEndpoint, msg.Id),
                new CancellationToken(),
                TaskCreationOptions.None,
                new CurrentThreadTaskScheduler());
            var pipe = new Mock<IDirectIncomingMessages>();
            {
                pipe.Setup(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()))
                    .Callback<EndpointId, MessageId, TimeSpan>(
                        (e, m, t) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreSame(msg.Id, m);
                            Assert.AreEqual(timeout, t);
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

            var response = layer.SendMessageAndWaitForResponse(remoteEndpoint, msg, retryCount, timeout);
            Assert.AreSame(responseTask, response);

            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(c => c.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
            pipe.Verify(p => p.ForwardResponse(It.IsAny<EndpointId>(), It.IsAny<MessageId>(), It.IsAny<TimeSpan>()), Times.Once());
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
                        It.IsAny<TaskScheduler>(), 
                        It.IsAny<int>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler, int>(
                        (e, p, c, s, r) =>
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
                () => layer.UploadData(endpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler(), 1));
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(
                    It.IsAny<ProtocolInformation>(), 
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>(), 
                    It.IsAny<TaskScheduler>(), 
                    It.IsAny<int>()),
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
                        It.IsAny<TaskScheduler>(), 
                        It.IsAny<int>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler, int>(
                        (e, p, c, s, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(file, p);
                        })
                    .Returns<ProtocolInformation, string, CancellationToken, TaskScheduler, int>(
                        (e, p, c, s, r) => Task.Factory.StartNew(() => { }, c, TaskCreationOptions.None, s))
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

            layer.UploadData(remoteEndpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler(), 1);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(
                    It.IsAny<ProtocolInformation>(),
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>(), 
                     It.IsAny<TaskScheduler>(), 
                     It.IsAny<int>()),
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
                        It.IsAny<TaskScheduler>(), 
                        It.IsAny<int>()))
                    .Callback<ProtocolInformation, string, CancellationToken, TaskScheduler, int>(
                        (e, p, c, s, r) =>
                        {
                            Assert.AreSame(endpointInfo.ProtocolInformation, e);
                            Assert.AreSame(file, p);
                        })
                    .Returns<ProtocolInformation, string, CancellationToken, TaskScheduler, int>(
                        (e, p, c, s, r) => Task.Factory.StartNew(() => { }, c, TaskCreationOptions.None, s))
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

            layer.UploadData(remoteEndpoint, file, new CancellationToken(), new CurrentThreadTaskScheduler(), 1);
            channel.Verify(c => c.OpenChannel(), Times.Once());
            channel.Verify(
                c => c.TransferData(
                    It.IsAny<ProtocolInformation>(),
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>(), 
                    It.IsAny<TaskScheduler>(), 
                    It.IsAny<int>()), 
                Times.Once());
        }
    }
}
