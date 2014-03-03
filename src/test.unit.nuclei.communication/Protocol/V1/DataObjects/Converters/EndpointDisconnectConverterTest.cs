//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointDisconnectConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new EndpointDisconnectConverter();
            Assert.AreEqual(typeof(EndpointDisconnectMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new EndpointDisconnectConverter();
            Assert.AreEqual(typeof(EndpointDisconnectData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new EndpointDisconnectConverter();

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
            var translator = new EndpointDisconnectConverter();

            var data = new EndpointDisconnectData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                DisconnectReason = "b",
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreSame(data.DisconnectReason, ((EndpointDisconnectMessage)msg).ClosingReason);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new EndpointDisconnectConverter();

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
            var translator = new EndpointDisconnectConverter();

            var msg = new EndpointDisconnectMessage(new EndpointId("a"), "b");
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(EndpointDisconnectData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreSame(msg.ClosingReason, ((EndpointDisconnectData)data).DisconnectReason);
        }
    }
}
