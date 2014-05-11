//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteCommandHubTest
    {
        [Test]
        public void HandleEndpointSignIn()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            SendMessageAndWaitForResponse sender =
                (e, m, r, t) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object, 
                new CommandProxyBuilder(
                    localEndpoint,
                    sender, 
                    configuration.Object,
                    systemDiagnostics), 
                systemDiagnostics);
            
            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).FullName,
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
                {
                    eventWasTriggered = true;
                    Assert.AreEqual(endpoint, e.Endpoint);
                    Assert.IsTrue(hub.HasCommandFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
                };

            hub.OnReceiptOfEndpointCommands(endpoint, types);
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void HandleEndpointSignOut()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            SendMessageAndWaitForResponse sender =
                (e, m, r, t) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    configuration.Object,
                    systemDiagnostics),
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).FullName,
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasCommandFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
            };
            hub.OnEndpointDisconnected += (s, e) =>
                {
                    eventWasTriggered = true;
                    Assert.AreEqual(endpoint, e.Endpoint);
                    Assert.IsFalse(hub.HasCommandFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
                };

            hub.OnReceiptOfEndpointCommands(endpoint, types);
            Assert.IsTrue(eventWasTriggered);

            eventWasTriggered = false;
            notifier.Raise(l => l.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void CommandsForWithUnknownCommand()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            SendMessageAndWaitForResponse sender =
                (e, m, r, t) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    configuration.Object,
                    systemDiagnostics),
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).FullName,
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasCommandFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
            };

            hub.OnReceiptOfEndpointCommands(endpoint, types);
            Assert.IsTrue(eventWasTriggered);
            
            var commands = hub.CommandsFor<InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn>(endpoint);
            Assert.IsNull(commands);
        }

        [Test]
        public void CommandsFor()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            SendMessageAndWaitForResponse sender =
                (e, m, r, t) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());

            var hub = new RemoteCommandHub(
                notifier.Object,
                new CommandProxyBuilder(
                    localEndpoint,
                    sender,
                    configuration.Object,
                    systemDiagnostics),
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).FullName,
                            typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasCommandFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
            };

            hub.OnReceiptOfEndpointCommands(endpoint, types);
            Assert.IsTrue(eventWasTriggered);

            var proxy = hub.CommandsFor<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>(endpoint);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<CommandSetProxy>(proxy);
            Assert.IsInstanceOf<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>(proxy);
        }
    }
}
