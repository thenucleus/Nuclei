//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
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
            var notifications = new Mock<IRaiseProxyNotifications>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new NotificationRaisedProcessAction(notifications.Object, systemDiagnostics);
            Assert.AreEqual(typeof(NotificationRaisedMessage), action.MessageTypeToProcess);
        }
        
        [Test]
        public void Invoke()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var remoteEndpoint = new EndpointId("other");
            var reg = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var eventArgs = new InteractionExtensionsTest.MySerializableEventArgs();
            var notifications = new Mock<IRaiseProxyNotifications>();
            {
                notifications.Setup(c => c.RaiseNotification(It.IsAny<EndpointId>(), It.IsAny<NotificationId>(), It.IsAny<EventArgs>()))
                    .Callback<EndpointId, NotificationId, EventArgs>(
                        (e, n, a) =>
                        {
                            Assert.AreSame(remoteEndpoint, e);
                            Assert.AreSame(reg, n);
                            Assert.AreSame(eventArgs, a);
                        })
                    .Verifiable();
            }

            var action = new NotificationRaisedProcessAction(notifications.Object, systemDiagnostics);
            action.Invoke(
                new NotificationRaisedMessage(
                    remoteEndpoint,
                    new NotificationRaisedData(reg, eventArgs)));

            notifications.Verify(n => n.RaiseNotification(It.IsAny<EndpointId>(), It.IsAny<NotificationId>(), It.IsAny<EventArgs>()), Times.Once());
        }
    }
}
