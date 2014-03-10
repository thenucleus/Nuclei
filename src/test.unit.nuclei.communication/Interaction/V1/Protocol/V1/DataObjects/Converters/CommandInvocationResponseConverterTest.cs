//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
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
    public sealed class CommandInvocationResponseConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationResponseConverter(serializers.Object);
            Assert.AreEqual(typeof(CommandInvokedResponseMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationResponseConverter(serializers.Object);
            Assert.AreEqual(typeof(CommandInvocationResponseData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationResponseConverter(serializers.Object);

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
                    .Callback<Type>(t => Assert.IsTrue(typeof(object).Equals(t)))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }

            var translator = new CommandInvocationResponseConverter(serializers.Object);

            var data = new CommandInvocationResponseData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                ReturnedType = new SerializedType
                    {
                        FullName = typeof(object).FullName,
                        AssemblyName = typeof(object).Assembly.GetName().Name
                    },
                Result = new object(),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(CommandInvokedResponseMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreEqual(data.Result, ((CommandInvokedResponseMessage)msg).Result);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationResponseConverter(serializers.Object);

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

            var translator = new CommandInvocationResponseConverter(serializers.Object);

            var msg = new CommandInvokedResponseMessage(
                new EndpointId("a"),
                new MessageId(), 
                new object());
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(NotificationRaisedData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(
                msg.Result.GetType().FullName, 
                ((CommandInvocationResponseData)data).ReturnedType.FullName);
            Assert.AreSame(msg.Result, ((CommandInvocationResponseData)data).Result);
        }
    }
}
