﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Communication.Messages;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
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
        public void Store()
        {
            var knownEndpoint = new EndpointId("other");
            var layer = new Mock<ISendDataViaChannels>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.KnownEndpoints())
                    .Returns(
                        new List<EndpointId> 
                            { 
                                knownEndpoint, 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            Type registeredType = null;
            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterNotificationType(It.IsAny<Type>()))
                    .Callback<Type>(t => registeredType = t)
                    .Verifiable();
            }

            var collection = new LocalNotificationCollection(layer.Object, store.Object);

            var obj = new MockNotificationSet();
            collection.Store(typeof(IMockNotificationSet), obj);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockNotificationSet)));
            Assert.AreEqual(typeof(IMockNotificationSet), registeredType);
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
            store.Verify(l => l.RegisterNotificationType(It.IsAny<Type>()), Times.Once());
        }

        [Test]
        public void StoreWithoutBeingSignedIn()
        {
            var layer = new Mock<ISendDataViaChannels>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(false);
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            Type registeredType = null;
            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterNotificationType(It.IsAny<Type>()))
                    .Callback<Type>(t => registeredType = t)
                    .Verifiable();
            }

            var collection = new LocalNotificationCollection(layer.Object, store.Object);

            var obj = new MockNotificationSet();
            collection.Store(typeof(IMockNotificationSet), obj);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockNotificationSet)));
            Assert.AreEqual(typeof(IMockNotificationSet), registeredType);
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
            store.Verify(l => l.RegisterNotificationType(It.IsAny<Type>()), Times.Once());
        }

        [Test]
        public void RaiseNormalEvent()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var layer = new Mock<ISendDataViaChannels>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.KnownEndpoints())
                    .Returns(
                        new List<EndpointId> 
                            { 
                                knownEndpoint, 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            other = e;
                            msg = m;
                        })
                    .Verifiable();
            }

            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterNotificationType(It.IsAny<Type>()))
                    .Verifiable();
            }

            var collection = new LocalNotificationCollection(layer.Object, store.Object);

            var obj = new MockNotificationSet();
            collection.Store(typeof(IMockNotificationSet), obj);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockNotificationSet)));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());

            var notificationType = ProxyExtensions.FromEventInfo(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            collection.RegisterForNotification(knownEndpoint, notificationType);

            var args = new EventArgs();
            obj.RaiseOnMyEvent(args);

            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            Assert.AreEqual(knownEndpoint, other);
            Assert.IsInstanceOf<NotificationRaisedMessage>(msg);

            var notificationMsg = msg as NotificationRaisedMessage;
            Assert.AreEqual(notificationType, notificationMsg.Notification);
            Assert.AreSame(args, notificationMsg.Arguments);
        }

        [Test]
        public void RaiseTypedEvent()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var layer = new Mock<ISendDataViaChannels>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.KnownEndpoints())
                    .Returns(
                        new List<EndpointId> 
                            { 
                                knownEndpoint, 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            other = e;
                            msg = m;
                        })
                    .Verifiable();
            }

            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterNotificationType(It.IsAny<Type>()))
                    .Verifiable();
            }

            var collection = new LocalNotificationCollection(layer.Object, store.Object);

            var obj = new MockNotificationSet();
            collection.Store(typeof(IMockNotificationSet), obj);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockNotificationSet)));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());

            var notificationType = ProxyExtensions.FromEventInfo(typeof(IMockNotificationSet).GetEvent("OnMyOtherEvent"));
            collection.RegisterForNotification(knownEndpoint, notificationType);

            var args = new UnhandledExceptionEventArgs(new Exception(), false);
            obj.RaiseOnMyOtherEvent(args);

            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            Assert.AreEqual(knownEndpoint, other);
            Assert.IsInstanceOf<NotificationRaisedMessage>(msg);

            var notificationMsg = msg as NotificationRaisedMessage;
            Assert.AreEqual(notificationType, notificationMsg.Notification);
            Assert.AreSame(args, notificationMsg.Arguments);
        }

        [Test]
        public void UnregisterFromNotification()
        {
            var knownEndpoint = new EndpointId("other");
            EndpointId other = null;
            ICommunicationMessage msg = null;
            var layer = new Mock<ISendDataViaChannels>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("mine"));
                layer.Setup(l => l.IsSignedIn)
                    .Returns(true);
                layer.Setup(l => l.KnownEndpoints())
                    .Returns(
                        new List<EndpointId> 
                            { 
                                knownEndpoint, 
                            });
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            other = e;
                            msg = m;
                        })
                    .Verifiable();
            }

            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterNotificationType(It.IsAny<Type>()))
                    .Verifiable();
            }

            var collection = new LocalNotificationCollection(layer.Object, store.Object);

            var obj = new MockNotificationSet();
            collection.Store(typeof(IMockNotificationSet), obj);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockNotificationSet)));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());

            var notificationType = ProxyExtensions.FromEventInfo(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            collection.RegisterForNotification(knownEndpoint, notificationType);
            collection.UnregisterFromNotification(knownEndpoint, notificationType);

            other = null;
            msg = null;

            var args = new EventArgs();
            obj.RaiseOnMyEvent(args);

            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
            Assert.IsNull(other);
            Assert.IsNull(msg);
        }
    }
}