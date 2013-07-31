//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class NotificationProxyBuilderTest
    {
        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonAssignableType()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(object)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(MockNotificationSetNotAnInterface)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithGenericParameters<>)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithProperties)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithMethods()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithMethods)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithoutEvents()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithoutEvents)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonEventHandlerEvent()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithNonEventHandlerEvent)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonSerializableEventArgs()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithNonSerializableEventArgs)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSet()
        {
            Assert.DoesNotThrow(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithEventHandler)));
            Assert.DoesNotThrow(
                () => NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(typeof(IMockNotificationSetWithTypedEventHandler)));
        }
        
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
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent += 
                (s, e) => 
                {
                    sender = s;
                    receivedArgs = e;
                };

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandler)), intermediateMsg.Notification.Type);
            Assert.AreEqual("OnMyEvent", intermediateMsg.Notification.MemberName);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

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
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithTypedEventHandler>(remoteEndpoint);

            object sender = null;
            EventArgs receivedArgs = null;
            proxy.OnMyEvent +=
                (s, e) =>
                {
                    sender = s;
                    receivedArgs = e;
                };

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockNotificationSetWithTypedEventHandler)), intermediateMsg.Notification.Type);
            Assert.AreEqual("OnMyEvent", intermediateMsg.Notification.MemberName);

            var notificationObj = proxy as NotificationSetProxy;
            Assert.IsNotNull(notificationObj);

            var args = new MySerializableEventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

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
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>(remoteEndpoint);

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

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            proxy.OnMyEvent -= handler;

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandler)), intermediateMsg.Notification.Type);
            Assert.AreEqual("OnMyEvent", intermediateMsg.Notification.MemberName);

            notificationObj.RaiseEvent("OnMyEvent", new EventArgs());
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
            var proxy = builder.ProxyConnectingTo<IMockNotificationSetWithEventHandler>(remoteEndpoint);

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

            var args = new EventArgs();
            notificationObj.RaiseEvent("OnMyEvent", args);

            Assert.AreSame(proxy, sender);
            Assert.AreSame(args, receivedArgs);

            sender = null;
            receivedArgs = null;
            notificationObj.ClearAllEvents();

            Assert.IsNull(intermediateMsg);

            notificationObj.RaiseEvent("OnMyEvent", new EventArgs());
            Assert.IsNull(sender);
            Assert.IsNull(receivedArgs);
        }
    }
}
