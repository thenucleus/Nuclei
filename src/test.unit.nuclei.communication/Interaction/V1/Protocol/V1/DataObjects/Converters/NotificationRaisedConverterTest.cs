//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1.DataObjects;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationRaisedConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new NotificationRaisedConverter(serializers.Object);
            Assert.AreEqual(typeof(NotificationRaisedMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new NotificationRaisedConverter(serializers.Object);
            Assert.AreEqual(typeof(NotificationRaisedData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new NotificationRaisedConverter(serializers.Object);

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
            var serializers = new Mock<IStoreObjectSerializers>();
            {
                serializers.Setup(s => s.HasSerializerFor(It.IsAny<Type>()))
                    .Callback<Type>(t => Assert.IsTrue(typeof(int).Equals(t)))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }
        
            var translator = new NotificationRaisedConverter(serializers.Object);

            var data = new NotificationRaisedData
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
                EventArgumentsType = new SerializedType
                    {
                        FullName = typeof(int).FullName,
                        AssemblyName = typeof(int).Assembly.GetName().Name,
                    },
                EventArguments = new EventArgs(),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(NotificationRaisedMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreEqual(data.InterfaceType.FullName, ((NotificationRaisedMessage)msg).Notification.Notification.InterfaceType.FullName);
            Assert.AreEqual(
                data.InterfaceType.AssemblyName, 
                ((NotificationRaisedMessage)msg).Notification.Notification.InterfaceType.Assembly.GetName().Name);
            Assert.AreSame(data.EventName, ((NotificationRaisedMessage)msg).Notification.Notification.EventName);
            Assert.AreSame(data.EventArguments, ((NotificationRaisedMessage)msg).Notification.EventArgs);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new NotificationRaisedConverter(serializers.Object);

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
            var serializers = new Mock<IStoreObjectSerializers>();
            {
                serializers.Setup(s => s.HasSerializerFor(It.IsAny<Type>()))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }
        
            var translator = new NotificationRaisedConverter(serializers.Object);

            var msg = new NotificationRaisedMessage(
                new EndpointId("a"),
                new Interaction.NotificationRaisedData(
                    new NotificationData(
                        typeof(int),
                        "event"),
                        new EventArgs()));
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(NotificationRaisedData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(msg.Notification.Notification.InterfaceType.FullName, ((NotificationRaisedData)data).InterfaceType.FullName);
            Assert.AreEqual(
                msg.Notification.Notification.InterfaceType.Assembly.GetName().Name,
                ((NotificationRaisedData)data).InterfaceType.AssemblyName);
            Assert.AreSame(msg.Notification.Notification.EventName, ((NotificationRaisedData)data).EventName);
            Assert.AreEqual(
                msg.Notification.EventArgs.GetType().FullName, 
                ((NotificationRaisedData)data).EventArgumentsType.FullName);
            Assert.AreEqual(
                msg.Notification.EventArgs.GetType().Assembly.GetName().Name,
                ((NotificationRaisedData)data).EventArgumentsType.AssemblyName);
            Assert.AreSame(msg.Notification.EventArgs, ((NotificationRaisedData)data).EventArguments);
        }
    }
}
