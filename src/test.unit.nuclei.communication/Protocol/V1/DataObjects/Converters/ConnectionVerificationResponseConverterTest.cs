//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Communication.Protocol.Messages;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ConnectionVerificationResponseConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationResponseConverter(serializers.Object);
            Assert.AreEqual(typeof(ConnectionVerificationResponseMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationResponseConverter(serializers.Object);
            Assert.AreEqual(typeof(ConnectionVerificationResponseData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationResponseConverter(serializers.Object);

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
                    .Callback<Type>(t => Assert.IsTrue(typeof(double).Equals(t)))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }

            var translator = new ConnectionVerificationResponseConverter(serializers.Object);

            var data = new ConnectionVerificationResponseData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                DataType = new SerializedType
                {
                    FullName = typeof(double).FullName,
                    AssemblyName = typeof(double).Assembly.GetName().Name
                },
                ResponseData = 1.0
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(ConnectionVerificationResponseMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreEqual(data.DataType.FullName, ((ConnectionVerificationResponseMessage)msg).ResponseData.GetType().FullName);
            Assert.AreEqual(data.DataType.AssemblyName, ((ConnectionVerificationResponseMessage)msg).ResponseData.GetType().Assembly.GetName().Name);
            Assert.AreEqual(1, ((ConnectionVerificationResponseMessage)msg).ResponseData);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationResponseConverter(serializers.Object);

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

            var translator = new ConnectionVerificationResponseConverter(serializers.Object);

            var msg = new ConnectionVerificationResponseMessage(new EndpointId("a"), new MessageId(), 1.0);
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(ConnectionVerificationResponseData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(typeof(double).FullName, ((ConnectionVerificationResponseData)data).DataType.FullName);
            Assert.AreEqual(typeof(double).Assembly.GetName().Name, ((ConnectionVerificationResponseData)data).DataType.AssemblyName);
            Assert.AreEqual(1.0, ((ConnectionVerificationResponseData)data).ResponseData);
        }
    }
}
