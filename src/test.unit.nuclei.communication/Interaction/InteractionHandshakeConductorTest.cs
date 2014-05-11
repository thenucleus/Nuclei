//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InteractionHandshakeConductorTest
    {
        [Test]
        public void HandshakeWithSuccessResponseWithLocalSendFirst()
        {
            var id = new EndpointId("a:10");
            var remoteEndpoint = new EndpointId("b:10");
            var endpointStorage = new Mock<IStoreInformationAboutEndpoints>();

            var providedSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("a"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("a"), 
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                                            new Version(1, 0))), 
                                },
                            new VersionedTypeFallback[0])
                    }
                };
            var requiredSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("b"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("b"), 
                            new VersionedTypeFallback[0],
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                                            new Version(1, 0))),
                                })
                    }
                };
            var interactionSubjects = new Mock<IStoreInteractionSubjects>();
            {
                interactionSubjects.Setup(i => i.ProvidedSubjects())
                    .Returns(providedSubjects.Keys);
                interactionSubjects.Setup(i => i.RequiredSubjects())
                    .Returns(requiredSubjects.Keys);
                interactionSubjects.Setup(i => i.ContainsGroupProvisionsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(providedSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupProvisionsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => providedSubjects[c]);
                interactionSubjects.Setup(i => i.ContainsGroupRequirementsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(requiredSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupRequirementsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => requiredSubjects[c]);
            }

            var commandProxies = new Mock<IStoreRemoteCommandProxies>();
            {
                commandProxies.Setup(
                        c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var notificationProxies = new Mock<IStoreRemoteNotificationProxies>();
            {
                notificationProxies.Setup(
                        n => n.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var wasMessageSend = false;
            SendMessage sendMessage = (endpoint, message, retries) =>
                {
                    wasMessageSend = true;

                    var msg = message as EndpointInteractionInformationResponseMessage;
                    Assert.IsNotNull(msg);
                    Assert.AreEqual(InteractionConnectionState.Desired, msg.State);
                };

            var wasMessageSendAndWaitedForResponse = false;
            SendMessageAndWaitForResponse sendMessageAndWaitForResponse = (endpoint, message, retries, timeout) =>
                {
                    wasMessageSendAndWaitedForResponse = true;

                    return Task<ICommunicationMessage>.Factory.StartNew(
                        () => new EndpointInteractionInformationResponseMessage(remoteEndpoint, new MessageId(), InteractionConnectionState.Neutral),
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var conductor = new InteractionHandshakeConductor(
                id,
                endpointStorage.Object,
                interactionSubjects.Object,
                commandProxies.Object,
                notificationProxies.Object,
                sendMessage,
                sendMessageAndWaitForResponse,
                TimeSpan.FromMinutes(1),
                diagnostics);

            endpointStorage.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(remoteEndpoint));
            Assert.IsTrue(wasMessageSendAndWaitedForResponse);
            Assert.IsFalse(wasMessageSend);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());

            conductor.ContinueHandshakeWith(
                remoteEndpoint, 
                requiredSubjects.Values.ToArray(),
                new MessageId());
            Assert.IsTrue(wasMessageSend);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Once());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Once());
        }

        [Test]
        public void HandshakeWithSuccessResponseWithRemoteSendFirst()
        {
            var id = new EndpointId("a:10");
            var remoteEndpoint = new EndpointId("b:10");
            var endpointStorage = new Mock<IStoreInformationAboutEndpoints>();

            var providedSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("a"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("a"), 
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                                            new Version(1, 0))), 
                                },
                            new VersionedTypeFallback[0])
                    }
                };
            var requiredSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("b"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("b"), 
                            new VersionedTypeFallback[0],
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                                            new Version(1, 0))),
                                })
                    }
                };
            var interactionSubjects = new Mock<IStoreInteractionSubjects>();
            {
                interactionSubjects.Setup(i => i.ProvidedSubjects())
                    .Returns(providedSubjects.Keys);
                interactionSubjects.Setup(i => i.RequiredSubjects())
                    .Returns(requiredSubjects.Keys);
                interactionSubjects.Setup(i => i.ContainsGroupProvisionsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(providedSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupProvisionsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => providedSubjects[c]);
                interactionSubjects.Setup(i => i.ContainsGroupRequirementsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(requiredSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupRequirementsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => requiredSubjects[c]);
            }

            var commandProxies = new Mock<IStoreRemoteCommandProxies>();
            {
                commandProxies.Setup(
                        c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var notificationProxies = new Mock<IStoreRemoteNotificationProxies>();
            {
                notificationProxies.Setup(
                        n => n.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var wasMessageSend = false;
            SendMessage sendMessage = (endpoint, message, retries) =>
            {
                wasMessageSend = true;

                var msg = message as EndpointInteractionInformationResponseMessage;
                Assert.IsNotNull(msg);
                Assert.AreEqual(InteractionConnectionState.Desired, msg.State);
            };

            var wasMessageSendAndWaitedForResponse = false;
            SendMessageAndWaitForResponse sendMessageAndWaitForResponse = (endpoint, message, retries, timeout) =>
            {
                wasMessageSendAndWaitedForResponse = true;

                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new EndpointInteractionInformationResponseMessage(remoteEndpoint, new MessageId(), InteractionConnectionState.Neutral),
                    new CancellationTokenSource().Token,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var conductor = new InteractionHandshakeConductor(
                id,
                endpointStorage.Object,
                interactionSubjects.Object,
                commandProxies.Object,
                notificationProxies.Object,
                sendMessage,
                sendMessageAndWaitForResponse,
                TimeSpan.FromMinutes(1),
                diagnostics);

            conductor.ContinueHandshakeWith(
                remoteEndpoint, 
                requiredSubjects.Values.ToArray(),
                new MessageId());
            Assert.IsTrue(wasMessageSend);
            Assert.IsTrue(wasMessageSendAndWaitedForResponse);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Once());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Once());
        }

        [Test]
        public void HandshakeWithRemoteRejection()
        {
            var id = new EndpointId("a:10");
            var remoteEndpoint = new EndpointId("b:10");
            var endpointStorage = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpointStorage.Setup(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true)
                    .Verifiable();
            }

            var providedSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("a"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("a"), 
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                                            new Version(1, 0))), 
                                },
                            new VersionedTypeFallback[0])
                    }
                };
            var requiredSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("b"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("b"), 
                            new VersionedTypeFallback[0],
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                                            new Version(1, 0))),
                                })
                    }
                };
            var interactionSubjects = new Mock<IStoreInteractionSubjects>();
            {
                interactionSubjects.Setup(i => i.ProvidedSubjects())
                    .Returns(providedSubjects.Keys);
                interactionSubjects.Setup(i => i.RequiredSubjects())
                    .Returns(requiredSubjects.Keys);
                interactionSubjects.Setup(i => i.ContainsGroupProvisionsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(providedSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupProvisionsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => providedSubjects[c]);
                interactionSubjects.Setup(i => i.ContainsGroupRequirementsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(requiredSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupRequirementsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => requiredSubjects[c]);
            }

            var commandProxies = new Mock<IStoreRemoteCommandProxies>();
            {
                commandProxies.Setup(
                        c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var notificationProxies = new Mock<IStoreRemoteNotificationProxies>();
            {
                notificationProxies.Setup(
                        n => n.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var wasMessageSend = false;
            SendMessage sendMessage = (endpoint, message, retries) =>
            {
                wasMessageSend = true;

                var msg = message as EndpointInteractionInformationResponseMessage;
                Assert.IsNotNull(msg);
                Assert.AreEqual(InteractionConnectionState.Neutral, msg.State);
            };

            var wasMessageSendAndWaitedForResponse = false;
            SendMessageAndWaitForResponse sendMessageAndWaitForResponse = (endpoint, message, retries, timeout) =>
            {
                wasMessageSendAndWaitedForResponse = true;

                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new EndpointInteractionInformationResponseMessage(remoteEndpoint, new MessageId(), InteractionConnectionState.Neutral),
                    new CancellationTokenSource().Token,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var conductor = new InteractionHandshakeConductor(
                id,
                endpointStorage.Object,
                interactionSubjects.Object,
                commandProxies.Object,
                notificationProxies.Object,
                sendMessage,
                sendMessageAndWaitForResponse,
                TimeSpan.FromMinutes(1),
                diagnostics);

            conductor.ContinueHandshakeWith(
                remoteEndpoint,
                new[]
                    {
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("b"),
                            new VersionedTypeFallback[0], 
                            new VersionedTypeFallback[0]), 
                    }, 
                new MessageId());
            Assert.IsTrue(wasMessageSend);
            Assert.IsTrue(wasMessageSendAndWaitedForResponse);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            endpointStorage.Verify(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()), Times.Once());
        }

        [Test]
        public void HandshakeWithLocalRejection()
        {
            var id = new EndpointId("a:10");
            var remoteEndpoint = new EndpointId("b:10");
            var endpointStorage = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpointStorage.Setup(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true)
                    .Verifiable();
            }

            var providedSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("a"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("a"), 
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                                            new Version(1, 0))), 
                                },
                            new VersionedTypeFallback[0])
                    }
                };
            var requiredSubjects = new Dictionary<CommunicationSubject, CommunicationSubjectGroup>
                {
                    {
                        new CommunicationSubject("b"), 
                        new CommunicationSubjectGroup(
                            new CommunicationSubject("b"), 
                            new VersionedTypeFallback[0],
                            new[]
                                {
                                    new VersionedTypeFallback(
                                        new Tuple<OfflineTypeInformation, Version>(
                                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                                            new Version(1, 0))),
                                })
                    }
                };
            var interactionSubjects = new Mock<IStoreInteractionSubjects>();
            {
                interactionSubjects.Setup(i => i.ProvidedSubjects())
                    .Returns(providedSubjects.Keys);
                interactionSubjects.Setup(i => i.RequiredSubjects())
                    .Returns(requiredSubjects.Keys);
                interactionSubjects.Setup(i => i.ContainsGroupProvisionsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(providedSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupProvisionsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => providedSubjects[c]);
                interactionSubjects.Setup(i => i.ContainsGroupRequirementsForSubject(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(requiredSubjects.ContainsKey);
                interactionSubjects.Setup(i => i.GroupRequirementsFor(It.IsAny<CommunicationSubject>()))
                    .Returns<CommunicationSubject>(c => requiredSubjects[c]);
            }

            var commandProxies = new Mock<IStoreRemoteCommandProxies>();
            {
                commandProxies.Setup(
                        c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var notificationProxies = new Mock<IStoreRemoteNotificationProxies>();
            {
                notificationProxies.Setup(
                        n => n.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()))
                    .Verifiable();
            }

            var wasMessageSend = false;
            SendMessage sendMessage = (endpoint, message, retries) =>
            {
                wasMessageSend = true;

                var msg = message as EndpointInteractionInformationResponseMessage;
                Assert.IsNotNull(msg);
                Assert.AreEqual(InteractionConnectionState.Neutral, msg.State);
            };

            var wasMessageSendAndWaitedForResponse = false;
            SendMessageAndWaitForResponse sendMessageAndWaitForResponse = (endpoint, message, retries, timeout) =>
            {
                wasMessageSendAndWaitedForResponse = true;

                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new EndpointInteractionInformationResponseMessage(remoteEndpoint, new MessageId(), InteractionConnectionState.Neutral),
                    new CancellationTokenSource().Token,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var conductor = new InteractionHandshakeConductor(
                id,
                endpointStorage.Object,
                interactionSubjects.Object,
                commandProxies.Object,
                notificationProxies.Object,
                sendMessage,
                sendMessageAndWaitForResponse,
                TimeSpan.FromMinutes(1),
                diagnostics);

            endpointStorage.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(remoteEndpoint));
            Assert.IsTrue(wasMessageSendAndWaitedForResponse);
            Assert.IsFalse(wasMessageSend);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());

            conductor.ContinueHandshakeWith(
                remoteEndpoint,
                new[]
                    {
                        new CommunicationSubjectGroup(new CommunicationSubject("b"), new VersionedTypeFallback[0], new VersionedTypeFallback[0]), 
                    },
                new MessageId());
            Assert.IsTrue(wasMessageSend);
            commandProxies.Verify(
                c => c.OnReceiptOfEndpointCommands(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            notificationProxies.Verify(
                c => c.OnReceiptOfEndpointNotifications(It.IsAny<EndpointId>(), It.IsAny<IEnumerable<OfflineTypeInformation>>()),
                Times.Never());
            endpointStorage.Verify(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()), Times.Once());
        }
    }
}
