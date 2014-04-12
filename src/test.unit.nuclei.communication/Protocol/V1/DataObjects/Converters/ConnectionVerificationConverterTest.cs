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
    public sealed class ConnectionVerificationConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationConverter(serializers.Object);
            Assert.AreEqual(typeof(ConnectionVerificationMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationConverter(serializers.Object);
            Assert.AreEqual(typeof(ConnectionVerificationData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationConverter(serializers.Object);

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

            var translator = new ConnectionVerificationConverter(serializers.Object);

            var data = new ConnectionVerificationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                DataType = new SerializedType
                    {
                        FullName = typeof(double).FullName,
                        AssemblyName = typeof(double).Assembly.GetName().Name
                    },
                CustomData = 1.0
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(ConnectionVerificationMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreEqual(data.DataType.FullName, ((ConnectionVerificationMessage)msg).CustomData.GetType().FullName);
            Assert.AreEqual(data.DataType.AssemblyName, ((ConnectionVerificationMessage)msg).CustomData.GetType().Assembly.GetName().Name);
            Assert.AreEqual(1, ((ConnectionVerificationMessage)msg).CustomData);
        }

        [Test]
        public void ToMessageWithNullCustomData()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            {
                serializers.Setup(s => s.HasSerializerFor(It.IsAny<Type>()))
                    .Callback<Type>(t => Assert.IsTrue(typeof(double).Equals(t)))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }

            var translator = new ConnectionVerificationConverter(serializers.Object);

            var data = new ConnectionVerificationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                DataType = new SerializedType
                {
                    FullName = typeof(object).FullName,
                    AssemblyName = typeof(object).Assembly.GetName().Name
                },
                CustomData = null
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(ConnectionVerificationMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.IsNull(((ConnectionVerificationMessage)msg).CustomData);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new ConnectionVerificationConverter(serializers.Object);

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

            var translator = new ConnectionVerificationConverter(serializers.Object);

            var msg = new ConnectionVerificationMessage(new EndpointId("a"), 1.0);
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(ConnectionVerificationData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(typeof(double).FullName, ((ConnectionVerificationData)data).DataType.FullName);
            Assert.AreEqual(typeof(double).Assembly.GetName().Name, ((ConnectionVerificationData)data).DataType.AssemblyName);
            Assert.AreEqual(1.0, ((ConnectionVerificationData)data).CustomData);
        }

        [Test]
        public void FromMessageWithNullCustomData()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            {
                serializers.Setup(s => s.HasSerializerFor(It.IsAny<Type>()))
                    .Returns(true);
                serializers.Setup(s => s.SerializerFor(It.IsAny<Type>()))
                    .Returns(new NonTransformingObjectSerializer());
            }

            var translator = new ConnectionVerificationConverter(serializers.Object);

            var msg = new ConnectionVerificationMessage(new EndpointId("a"));
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(ConnectionVerificationData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(typeof(object).FullName, ((ConnectionVerificationData)data).DataType.FullName);
            Assert.AreEqual(typeof(object).Assembly.GetName().Name, ((ConnectionVerificationData)data).DataType.AssemblyName);
            Assert.IsNull(((ConnectionVerificationData)data).CustomData);
        }
    }
}
