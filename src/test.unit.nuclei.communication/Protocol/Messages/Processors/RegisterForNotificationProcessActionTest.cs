//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Communication.Interaction;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RegisterForNotificationProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

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
            ISerializedEventRegistration registration = null;
            var sink = new Mock<ISendNotifications>();
            {
                sink.Setup(s => s.RegisterForNotification(It.IsAny<EndpointId>(), It.IsAny<ISerializedEventRegistration>()))
                    .Callback<EndpointId, ISerializedEventRegistration>((e, s) =>
                    {
                        processedId = e;
                        registration = s;
                    });
            }

            var action = new RegisterForNotificationProcessAction(sink.Object);

            var id = new EndpointId("id");
            ISerializedEventRegistration reg =
                ProxyExtensions.FromEventInfo(typeof(IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var msg = new RegisterForNotificationMessage(id, reg);
            action.Invoke(msg);

            Assert.AreEqual(id, processedId);
            Assert.AreEqual(reg, registration);
        }
    }
}
