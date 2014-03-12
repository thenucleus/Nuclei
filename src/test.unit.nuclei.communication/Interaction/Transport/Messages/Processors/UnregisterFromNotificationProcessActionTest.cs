//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class UnregisterFromNotificationProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<ISendNotifications>();

            var action = new UnregisterFromNotificationProcessAction(sink.Object);
            Assert.AreEqual(typeof(UnregisterFromNotificationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointId processedId = null;
            NotificationData registration = null;
            var sink = new Mock<ISendNotifications>();
            {
                sink.Setup(s => s.UnregisterFromNotification(It.IsAny<EndpointId>(), It.IsAny<NotificationData>()))
                    .Callback<EndpointId, NotificationData>((e, s) =>
                    {
                        processedId = e;
                        registration = s;
                    });
            }

            var action = new UnregisterFromNotificationProcessAction(sink.Object);

            var id = new EndpointId("id");
            NotificationData reg = new NotificationData(typeof(IMockNotificationSetWithTypedEventHandler), "OnMyEvent");
            var msg = new UnregisterFromNotificationMessage(id, reg);
            action.Invoke(msg);

            Assert.AreEqual(id, processedId);
            Assert.AreEqual(reg, registration);
        }
    }
}
