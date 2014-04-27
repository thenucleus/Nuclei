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
    public sealed class RegisterForNotificationProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<ISendNotifications>();

            var action = new RegisterForNotificationProcessAction(sink.Object);
            Assert.AreEqual(typeof(RegisterForNotificationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointId processedId = null;
            NotificationId registration = null;
            var sink = new Mock<ISendNotifications>();
            {
                sink.Setup(s => s.RegisterForNotification(It.IsAny<EndpointId>(), It.IsAny<NotificationId>()))
                    .Callback<EndpointId, NotificationId>((e, s) =>
                    {
                        processedId = e;
                        registration = s;
                    });
            }

            var action = new RegisterForNotificationProcessAction(sink.Object);

            var id = new EndpointId("id");
            var reg = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var msg = new RegisterForNotificationMessage(id, reg);
            action.Invoke(msg);

            Assert.AreEqual(id, processedId);
            Assert.AreEqual(reg, registration);
        }
    }
}
