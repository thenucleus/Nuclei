//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol.Messages;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SuccessConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new SuccessConverter();
            Assert.AreEqual(typeof(SuccessMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new SuccessConverter();
            Assert.AreEqual(typeof(SuccessData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new SuccessConverter();

            var data = new FailureData
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
            var translator = new SuccessConverter();

            var data = new SuccessData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(SuccessMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new SuccessConverter();

            var msg = new FailureMessage(new EndpointId("a"), new MessageId());
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(UnknownMessageTypeData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
        }

        [Test]
        public void FromMessage()
        {
            var translator = new SuccessConverter();

            var msg = new SuccessMessage(new EndpointId("a"), new MessageId());
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(SuccessData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
        }
    }
}
