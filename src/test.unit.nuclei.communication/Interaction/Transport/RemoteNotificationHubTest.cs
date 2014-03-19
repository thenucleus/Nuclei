//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteNotificationHubTest
    {
        [Test]
        public void HandleEndpointSignIn()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics), 
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).FullName,
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasNotificationFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler)));
            };

            hub.OnReceiptOfEndpointNotifications(endpoint, types);
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void HandleEndpointSignOut()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics), 
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).FullName,
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasNotificationFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler)));
            };
            hub.OnEndpointDisconnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsFalse(hub.HasNotificationFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler)));
            };

            hub.OnReceiptOfEndpointNotifications(endpoint, types);
            Assert.IsTrue(eventWasTriggered);

            eventWasTriggered = false;
            notifier.Raise(n => n.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void NotificationsForWithUnknownNotification()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics),
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).FullName,
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).Assembly.GetName())
                    };

            var eventWasTriggered = false;
            hub.OnEndpointConnected += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.AreEqual(endpoint, e.Endpoint);
                Assert.IsTrue(hub.HasNotificationFor(e.Endpoint, typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler)));
            };

            hub.OnReceiptOfEndpointNotifications(endpoint, types);
            Assert.IsTrue(eventWasTriggered);

            Assert.Throws<NotificationNotSupportedException>(
                () => hub.NotificationsFor<InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler>(endpoint));
        }

        [Test]
        public void NotificationsFor()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<IStoreInformationAboutEndpoints>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics),
                systemDiagnostics);

            var endpoint = new EndpointId("other");
            var types = new List<OfflineTypeInformation>
                    {
                        new OfflineTypeInformation(
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).FullName,
                            typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).Assembly.GetName())
                    };

            hub.OnReceiptOfEndpointNotifications(endpoint, types);

            var proxy = hub.NotificationsFor<InteractionExtensionsTest.IMockNotificationSetWithEventHandler>(endpoint);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<NotificationSetProxy>(proxy);
            Assert.IsInstanceOf<InteractionExtensionsTest.IMockNotificationSetWithEventHandler>(proxy);
        }
    }
}
