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

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationRaisedProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var commands = new Mock<INotifyOfRemoteEndpointEvents>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new NotificationRaisedProcessAction(commands.Object, systemDiagnostics);
            Assert.AreEqual(typeof(NotificationRaisedMessage), action.MessageTypeToProcess);
        }
        
        [Test]
        public void Invoke()
        {
            var local = new EndpointId("local");
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) => { };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new NotificationProxyBuilder(local, messageSender, systemDiagnostics);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo(remoteEndpoint, typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler));

            object sender = null;
            EventArgs args = null;
            ((InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler)proxy).OnMyEvent += 
                (s, e) => 
                {
                    sender = s;
                    args = e;
                };

            var commandSets = new List<KeyValuePair<Type, INotificationSet>> 
                { 
                    new KeyValuePair<Type, INotificationSet>(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler), proxy)
                };

            var notifications = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notifications.Setup(c => c.NotificationsFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
            }

            var action = new NotificationRaisedProcessAction(notifications.Object, systemDiagnostics);

            var eventArgs = new InteractionExtensionsTest.MySerializableEventArgs();
            action.Invoke(
                new NotificationRaisedMessage(
                    new EndpointId("otherId"),
                    new NotificationRaisedData(
                        new NotificationData(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler), "OnMyEvent"), eventArgs)));

            Assert.AreSame(proxy, sender);
            Assert.AreSame(eventArgs, args);
        }
    }
}
