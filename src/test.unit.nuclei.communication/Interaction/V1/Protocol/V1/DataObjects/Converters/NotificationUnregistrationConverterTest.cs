//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1.DataObjects;

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

            var data = new NotificationUnregistrationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                InterfaceType = new SerializedType
                    {
                        FullName = typeof(int).FullName,
                        AssemblyName = typeof(int).Assembly.GetName().Name
                    },
                EventName = "event",
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(UnregisterFromNotificationMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreSame(data.InterfaceType, ((UnregisterFromNotificationMessage)msg).Notification.InterfaceType.FullName);
            Assert.AreSame(data.EventName, ((UnregisterFromNotificationMessage)msg).Notification.EventName);
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

            var msg = new UnregisterFromNotificationMessage(
                new EndpointId("a"), 
                new NotificationData(
                    typeof(int),
                    "event"));
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(DownloadRequestData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreSame(msg.Notification.InterfaceType.FullName, ((NotificationUnregistrationData)data).InterfaceType.FullName);
            Assert.AreSame(
                msg.Notification.InterfaceType.Assembly.GetName().Name, 
                ((NotificationUnregistrationData)data).InterfaceType.AssemblyName);
            Assert.AreSame(msg.Notification.EventName, ((NotificationUnregistrationData)data).EventName);
        }
    }
}
