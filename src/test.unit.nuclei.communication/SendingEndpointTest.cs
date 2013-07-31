//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Moq;
using Nuclei.Communication.Messages;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SendingEndpointTest
    {
        [Test]
        public void SendMessageToUnknownReceiver()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var messageProxy = new Mock<IMessageSendingEndpoint>();
            {
                messageProxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(input => Assert.AreSame(msg, input));
            }

            var dataProxy = new Mock<IDataTransferingEndpoint>();
            {
                dataProxy.Setup(p => p.Send(It.IsAny<DataTransferMessage>()))
                    .Verifiable();
            }

            var localEndpoint = new EndpointId("local");
            Func<EndpointId, IMessageSendingEndpoint> builder = id => messageProxy.Object;
            Func<EndpointId, IDataTransferingEndpoint> dataBuilder = id => dataProxy.Object;
            var sender = new SendingEndpoint(localEndpoint, builder, dataBuilder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Never());
        }

        [Test]
        public void SendMessageToKnownReceiver()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var messageProxy = new Mock<IMessageSendingEndpoint>();
            {
                messageProxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(input => Assert.AreSame(msg, input))
                    .Verifiable();
            }

            var dataProxy = new Mock<IDataTransferingEndpoint>();
            {
                dataProxy.Setup(p => p.Send(It.IsAny<DataTransferMessage>()))
                    .Verifiable();
            }

            var localEndpoint = new EndpointId("local");
            Func<EndpointId, IMessageSendingEndpoint> builder = id => messageProxy.Object;
            Func<EndpointId, IDataTransferingEndpoint> dataBuilder = id => dataProxy.Object;
            var sender = new SendingEndpoint(localEndpoint, builder, dataBuilder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            messageProxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Exactly(1));
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Never());

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            messageProxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Exactly(2));
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Never());
        }

        [Test]
        public void SendDataToUnknownReceiver()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            data.Position = 0;

            var localEndpoint = new EndpointId("local");
            var endpointId = new EndpointId("id");
            var messageProxy = new Mock<IMessageSendingEndpoint>();
            {
                messageProxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            var dataProxy = new Mock<IDataTransferingEndpoint>();
            {
                dataProxy.Setup(p => p.Send(It.IsAny<DataTransferMessage>()))
                    .Callback<DataTransferMessage>(
                        msg =>
                        {
                            Assert.AreSame(localEndpoint, msg.SendingEndpoint);
                            Assert.AreSame(endpointId, msg.ReceivingEndpoint);
                            Assert.AreSame(data, msg.Data);
                        })
                    .Verifiable();
            }

            Func<EndpointId, IMessageSendingEndpoint> builder = id => messageProxy.Object;
            Func<EndpointId, IDataTransferingEndpoint> dataBuilder = id => dataProxy.Object;
            var sender = new SendingEndpoint(localEndpoint, builder, dataBuilder);

            sender.Send(endpointId, data);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            messageProxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Never());
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Exactly(1));
        }

        [Test]
        public void SendDataToKnownReceiver()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            data.Position = 0;

            var localEndpoint = new EndpointId("local");
            var endpointId = new EndpointId("id");
            var messageProxy = new Mock<IMessageSendingEndpoint>();
            {
                messageProxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Verifiable();
            }

            var dataProxy = new Mock<IDataTransferingEndpoint>();
            {
                dataProxy.Setup(p => p.Send(It.IsAny<DataTransferMessage>()))
                    .Callback<DataTransferMessage>(
                        msg =>
                        {
                            Assert.AreSame(localEndpoint, msg.SendingEndpoint);
                            Assert.AreSame(endpointId, msg.ReceivingEndpoint);
                            Assert.AreSame(data, msg.Data);
                        })
                    .Verifiable();
            }

            Func<EndpointId, IMessageSendingEndpoint> builder = id => messageProxy.Object;
            Func<EndpointId, IDataTransferingEndpoint> dataBuilder = id => dataProxy.Object;
            var sender = new SendingEndpoint(localEndpoint, builder, dataBuilder);

            sender.Send(endpointId, data);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            messageProxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Never());
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Exactly(1));

            sender.Send(endpointId, data);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            messageProxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Never());
            dataProxy.Verify(p => p.Send(It.IsAny<DataTransferMessage>()), Times.Exactly(2));
        }

        [Test]
        public void CloseChannelTo()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var messageProxy = new Mock<IMessageSendingEndpoint>();
            var messageDisposable = messageProxy.As<IDisposable>();

            var dataProxy = new Mock<IDataTransferingEndpoint>();
            var dataDisposable = dataProxy.As<IDisposable>();

            var localEndpoint = new EndpointId("local");
            Func<EndpointId, IMessageSendingEndpoint> messageBuilder = id => messageProxy.Object;
            Func<EndpointId, IDataTransferingEndpoint> dataBuilder = id => dataProxy.Object;
            var sender = new SendingEndpoint(localEndpoint, messageBuilder, dataBuilder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());

            sender.CloseChannelTo(endpointId);
            Assert.AreEqual(0, sender.KnownEndpoints().Count());
        }
    }
}
