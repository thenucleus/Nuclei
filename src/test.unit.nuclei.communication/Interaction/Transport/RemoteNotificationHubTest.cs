//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Communication.Protocol;
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
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(new List<CommunicationSubject>(), 
                new List<ISerializedType>(), 
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent)),
                    });

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            notifier.Raise(n => n.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void HandleEndpointSignOut()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent)),
                    });

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            hub.OnEndpointSignedOff += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsFalse(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsFalse(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            notifier.Raise(n => n.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);

            eventWasTriggered = false;
            notifier.Raise(n => n.OnEndpointDisconnected += null, new EndpointSignedOutEventArgs(connectionInfo.Id, connectionInfo.ChannelTemplate));
            Assert.IsTrue(eventWasTriggered);
        }

        [Test]
        public void NotificationsForWithUnknownNotification()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics),
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent)),
                    });

            var eventWasTriggered = false;
            hub.OnEndpointSignedIn += (s, e) =>
            {
                eventWasTriggered = true;
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            notifier.Raise(n => n.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));
            Assert.IsTrue(eventWasTriggered);

            Assert.Throws<NotificationNotSupportedException>(
                () => hub.NotificationsFor<IMockNotificationSetWithTypedEventHandlerEvent>(connectionInfo.Id));
        }

        [Test]
        public void NotificationsFor()
        {
            var localEndpoint = new EndpointId("local");
            var notifier = new Mock<INotifyOfEndpointStateChange>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteNotificationHub(
                notifier.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    systemDiagnostics),
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                ChannelTemplate.NamedPipe,
                new Uri("net.pipe://localhost/apollo_test"));
            var description = new CommunicationDescription(new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent)),
                    });
            notifier.Raise(n => n.OnEndpointConnected += null, new EndpointSignInEventArgs(connectionInfo, description));

            var proxy = hub.NotificationsFor<IMockNotificationSetWithEventHandlerEvent>(connectionInfo.Id);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<NotificationSetProxy>(proxy);
            Assert.IsInstanceOf<IMockNotificationSetWithEventHandlerEvent>(proxy);
        }
    }
}
