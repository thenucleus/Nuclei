//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class LocalNotificationCollectionTest
    {
        public interface IMockNotificationSet : INotificationSet
        {
            event EventHandler OnMyEvent;

            event EventHandler<UnhandledExceptionEventArgs> OnMyOtherEvent;
        }

        public sealed class MockNotificationSet : IMockNotificationSet
        {
            public event EventHandler OnMyEvent;

            public void RaiseOnMyEvent(EventArgs eventArgs)
            {
                var local = OnMyEvent;
                if (local != null)
                {
                    local(this, eventArgs);
                }
            }

            public event EventHandler<UnhandledExceptionEventArgs> OnMyOtherEvent;

            public void RaiseOnMyOtherEvent(UnhandledExceptionEventArgs eventArgs)
            {
                var local = OnMyOtherEvent;
                if (local != null)
                {
                    local(this, eventArgs);
                }
            }
        }

        [Test]
        public void Register()
        {
            var wasInvoked = false;
            SendMessage sender =
                (endpoint, message, retries) =>
                {
                    wasInvoked = true;
                };

            var collection = new LocalNotificationCollection(new EndpointId("a"), sender);

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            var def = new NotificationDefinition(id);
            collection.Register(new[] { def });

            Assert.IsTrue(collection.Any(pair => pair.Equals(id)));
            Assert.IsFalse(wasInvoked);
        }

        [Test]
        public void RegisterWithoutBeingSignedIn()
        {
            var wasInvoked = false;
            SendMessage sender =
                (endpoint, message, retries) =>
                {
                    wasInvoked = true;
                };

            var collection = new LocalNotificationCollection(new EndpointId("a"), sender);

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            var def = new NotificationDefinition(id);
            collection.Register(new[] { def });

            Assert.AreEqual(1, collection.Count(pair => pair.Equals(id)));
            Assert.IsFalse(wasInvoked);
        }

        [Test]
        public void RaiseNormalEvent()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var wasInvoked = false;
            SendMessage sender =
                (endpoint, message, retries) =>
                {
                    wasInvoked = true;
                    other = endpoint;
                    msg = message;
                };

            var collection = new LocalNotificationCollection(new EndpointId("a"), sender);

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            var def = new NotificationDefinition(id);
            var obj = new MockNotificationSet();
            obj.OnMyEvent += def.ForwardToListeners;
            collection.Register(new[] { def });

            Assert.AreEqual(1, collection.Count(pair => pair.Equals(id)));
            Assert.IsFalse(wasInvoked);

            collection.RegisterForNotification(knownEndpoint, id);

            var args = new EventArgs();
            obj.RaiseOnMyEvent(args);

            Assert.IsTrue(wasInvoked);
            Assert.AreEqual(knownEndpoint, other);
            Assert.IsInstanceOf<NotificationRaisedMessage>(msg);

            var notificationMsg = msg as NotificationRaisedMessage;
            Assert.AreEqual(id, notificationMsg.Notification.Notification);
            Assert.AreSame(args, notificationMsg.Notification.EventArgs);
        }

        [Test]
        public void RaiseTypedEvent()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var wasInvoked = false;
            SendMessage sender =
                (endpoint, message, retries) =>
                {
                    wasInvoked = true;
                    other = endpoint;
                    msg = message;
                };

            var collection = new LocalNotificationCollection(new EndpointId("a"), sender);

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyOtherEvent"));
            var def = new NotificationDefinition(id);
            var obj = new MockNotificationSet();
            obj.OnMyOtherEvent += def.ForwardToListeners;
            collection.Register(new[] { def });

            Assert.AreEqual(1, collection.Count(pair => pair.Equals(id)));
            Assert.IsFalse(wasInvoked);

            collection.RegisterForNotification(knownEndpoint, id);

            var args = new UnhandledExceptionEventArgs(new Exception(), false);
            obj.RaiseOnMyOtherEvent(args);

            Assert.IsTrue(wasInvoked);
            Assert.AreEqual(knownEndpoint, other);
            Assert.IsInstanceOf<NotificationRaisedMessage>(msg);

            var notificationMsg = msg as NotificationRaisedMessage;
            Assert.AreEqual(id, notificationMsg.Notification.Notification);
            Assert.AreSame(args, notificationMsg.Notification.EventArgs);
        }

        [Test]
        public void UnregisterFromNotification()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var wasInvoked = false;
            SendMessage sender =
                (endpoint, message, retries) =>
                {
                    wasInvoked = true;
                    other = endpoint;
                    msg = message;
                };

            var collection = new LocalNotificationCollection(new EndpointId("a"), sender);

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            var def = new NotificationDefinition(id);
            var obj = new MockNotificationSet();
            obj.OnMyEvent += def.ForwardToListeners;
            collection.Register(new[] { def });

            Assert.AreEqual(1, collection.Count(pair => pair.Equals(id)));
            Assert.IsFalse(wasInvoked);

            collection.RegisterForNotification(knownEndpoint, id);
            collection.UnregisterFromNotification(knownEndpoint, id);

            other = null;
            msg = null;

            var args = new EventArgs();
            obj.RaiseOnMyEvent(args);

            Assert.IsFalse(wasInvoked);
            Assert.IsNull(other);
            Assert.IsNull(msg);
        }
    }
}
