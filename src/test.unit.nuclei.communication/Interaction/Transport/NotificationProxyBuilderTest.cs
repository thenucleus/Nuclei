//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed class NotificationProxyBuilderTest
    {
        [Test]
        public void ProxyConnectingToEventWithNormalEventHandler()
        {
            var local = new EndpointId("local");
            RegisterForNotificationMessage intermediateMsg = null;
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) =>
            {
                intermediateMsg = m as RegisterForNotificationMessage;
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new NotificationProxyBuilder(local, messageSender, systemDiagnostics);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockNotificationSetWithEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent += 
                (s, e) => 
                {
                    sender = s;
                    receivedArgs = e;
                };

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).GetEvent("OnMyEvent"));
            Assert.AreEqual(id, intermediateMsg.Notification);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new EventArgs();
            notificationObj.RaiseEvent(id, args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);
        }

        [Test]
        public void ProxyConnectingToEventWithTypedEventHandler()
        {
            var local = new EndpointId("local");
            RegisterForNotificationMessage intermediateMsg = null;
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) =>
            {
                intermediateMsg = m as RegisterForNotificationMessage;
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new NotificationProxyBuilder(local, messageSender, systemDiagnostics);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent +=
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            Assert.AreEqual(id, intermediateMsg.Notification);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new InteractionExtensionsTest.MySerializableEventArgs();
            notificationObj.RaiseEvent(id, args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);
        }

        [Test]
        public void ProxyDisconnectFromEventWithNormalEventHandler()
        {
            var local = new EndpointId("local");
            UnregisterFromNotificationMessage intermediateMsg = null;
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) =>
            {
                intermediateMsg = m as UnregisterFromNotificationMessage;
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new NotificationProxyBuilder(local, messageSender, systemDiagnostics);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockNotificationSetWithEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;

            EventHandler handler = 
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };
            proxy.OnMyEvent += handler;

            Assert.IsNull(intermediateMsg);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).GetEvent("OnMyEvent"));
            var args = new EventArgs();
            notificationObj.RaiseEvent(id, args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            proxy.OnMyEvent -= handler;

            Assert.AreEqual(id, intermediateMsg.Notification);

            notificationObj.RaiseEvent(id, new EventArgs());
            Assert.IsNull(sender);
            Assert.IsNull(receivedArgs);
        }

        [Test]
        public void ProxyCleanupAttachedEvents()
        {
            var local = new EndpointId("local");
            UnregisterFromNotificationMessage intermediateMsg = null;
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) =>
            {
                intermediateMsg = m as UnregisterFromNotificationMessage;
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new NotificationProxyBuilder(local, messageSender, systemDiagnostics);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockNotificationSetWithEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent +=
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };

            Assert.IsNull(intermediateMsg);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler).GetEvent("OnMyEvent"));
            var args = new EventArgs();
            notificationObj.RaiseEvent(id, args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            notificationObj.ClearAllEvents();

            Assert.IsNull(intermediateMsg);

            notificationObj.RaiseEvent(id, new EventArgs());
            Assert.IsNull(sender);
            Assert.IsNull(receivedArgs);
        }
    }
}
