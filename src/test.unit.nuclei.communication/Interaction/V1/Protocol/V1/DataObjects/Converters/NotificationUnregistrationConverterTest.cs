//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationUnregistrationConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new NotificationUnregistrationConverter();
            Assert.AreEqual(typeof(UnregisterFromNotificationMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new NotificationUnregistrationConverter();
            Assert.AreEqual(typeof(NotificationUnregistrationData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new NotificationUnregistrationConverter();

            var data = new SuccessData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(UnknownMessageTypeMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
        }

        [Test]
        public void ToMessage()
        {
            var translator = new NotificationUnregistrationConverter();

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var data = new NotificationUnregistrationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                NotificationId = NotificationIdExtensions.Serialize(id),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(UnregisterFromNotificationMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreEqual(id, ((UnregisterFromNotificationMessage)msg).Notification);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new NotificationUnregistrationConverter();

            var msg = new SuccessMessage(new EndpointId("a"), new MessageId());
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(UnknownMessageTypeData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
        }

        [Test]
        public void FromMessage()
        {
            var translator = new NotificationUnregistrationConverter();

            var id = NotificationId.Create(typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var msg = new UnregisterFromNotificationMessage(new EndpointId("a"), id);

            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(NotificationUnregistrationData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(NotificationIdExtensions.Serialize(id), ((NotificationUnregistrationData)data).NotificationId);
        }
    }
}
