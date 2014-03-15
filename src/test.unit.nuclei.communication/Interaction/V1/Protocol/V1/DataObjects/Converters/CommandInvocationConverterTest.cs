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
    public sealed class CommandInvocationConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationConverter(serializers.Object);
            Assert.AreEqual(typeof(CommandInvokedMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationConverter(serializers.Object);
            Assert.AreEqual(typeof(CommandInvocationData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationConverter(serializers.Object);

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

            var translator = new CommandInvocationConverter(serializers.Object);

            var data = new CommandInvocationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                InterfaceType = new SerializedType
                {
                    FullName = typeof(int).FullName,
                    AssemblyName = typeof(int).Assembly.GetName().Name
                },
                MethodName = "method",
                ParameterTypes = new[]
                    {
                        new SerializedType
                            {
                                FullName = typeof(object).FullName,
                                AssemblyName = typeof(object).Assembly.GetName().Name
                            }, 
                    },
                ParameterValues = new object[]
                    {
                        1,
                    }
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(CommandInvokedMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreEqual(data.InterfaceType.FullName, ((CommandInvokedMessage)msg).Invocation.Command.InterfaceType.FullName);
            Assert.AreEqual(
                data.InterfaceType.AssemblyName,
                ((CommandInvokedMessage)msg).Invocation.Command.InterfaceType.Assembly.GetName().Name);
            Assert.AreSame(data.MethodName, ((CommandInvokedMessage)msg).Invocation.Command.MethodName);
            Assert.AreEqual(data.ParameterTypes.Length, ((CommandInvokedMessage)msg).Invocation.ParameterValues.Length);
            Assert.AreEqual(typeof(object), ((CommandInvokedMessage)msg).Invocation.ParameterValues[0].Item1);
            Assert.AreEqual(1, ((CommandInvokedMessage)msg).Invocation.ParameterValues[0].Item2);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var serializers = new Mock<IStoreObjectSerializers>();
            var translator = new CommandInvocationConverter(serializers.Object);

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

            var translator = new CommandInvocationConverter(serializers.Object);

            var msg = new CommandInvokedMessage(
                new EndpointId("a"),
                new CommandInvokedData(
                    new CommandData(
                        typeof(int),
                        "CompareTo"), 
                    new[]
                        {
                            new Tuple<Type, object>(typeof(object), 1), 
                        }));
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(CommandInvocationData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(msg.Invocation.Command.InterfaceType.FullName, ((CommandInvocationData)data).InterfaceType.FullName);
            Assert.AreEqual(
                msg.Invocation.Command.InterfaceType.Assembly.GetName().Name,
                ((CommandInvocationData)data).InterfaceType.AssemblyName);
            Assert.AreSame(msg.Invocation.Command.MethodName, ((CommandInvocationData)data).MethodName);
            Assert.AreEqual(msg.Invocation.ParameterValues.Length, ((CommandInvocationData)data).ParameterTypes.Length);
            Assert.AreEqual(
                msg.Invocation.ParameterValues[0].Item1.FullName, 
                ((CommandInvocationData)data).ParameterTypes[0].FullName);
            Assert.AreEqual(
                msg.Invocation.ParameterValues[0].Item1.Assembly.GetName().Name,
                ((CommandInvocationData)data).ParameterTypes[0].AssemblyName);
            Assert.AreEqual(msg.Invocation.ParameterValues.Length, ((CommandInvocationData)data).ParameterValues.Length);
            Assert.AreEqual(
                msg.Invocation.ParameterValues[0].Item2,
                ((CommandInvocationData)data).ParameterValues[0]);
        }
    }
}
