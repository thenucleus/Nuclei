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
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteCommandHubTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        public interface IMockCommandSetWithTypedTaskReturn : ICommandSet
        {
            Task<int> MyMethod(int input);
        }

        [Test]
        public void HandleEndpointSignIn()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object, 
                new CommandProxyBuilder(
                    localEndpoint,
                    sender, 
                    systemDiagnostics), 
                systemDiagnostics);
            
            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                ChannelTemplate.NamedPipe, 
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))
                    },
                new List<ISerializedType>());

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
                {
                    eventWasTriggered = true;
                    Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                };

            notifier.Raise(l => l.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void HandleEndpointSignOut()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    systemDiagnostics),
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe, 
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))
                    },
                new List<ISerializedType>());

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
                {
                    eventWasTriggered = true;
                    Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                };
            hub.OnEndpointSignedOff += (s, e) =>
                {
                    eventWasTriggered = true;
                    Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                };

            notifier.Raise(l => l.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);

            eventWasTriggered = false;
            notifier.Raise(l => l.OnEndpointDisconnected += null, new EndpointSignedOutEventArgs(connectionInfo.Id, connectionInfo.ChannelTemplate));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void CommandsForWithUnknownCommand()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    systemDiagnostics),
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))
                    },
                new List<ISerializedType>());

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            };

            notifier.Raise(l => l.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);
            
            var commands = hub.CommandsFor<IMockCommandSetWithTypedTaskReturn>(connectionInfo.Id);
            Assert.IsNull(commands);
        }

        [Test]
        public void CommandsFor()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    systemDiagnostics),
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))
                    },
                new List<ISerializedType>());

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            };

            notifier.Raise(l => l.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);

            var proxy = hub.CommandsFor<IMockCommandSetWithTaskReturn>(connectionInfo.Id);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<CommandSetProxy>(proxy);
            Assert.IsInstanceOf<IMockCommandSetWithTaskReturn>(proxy);
        }
    }
}
