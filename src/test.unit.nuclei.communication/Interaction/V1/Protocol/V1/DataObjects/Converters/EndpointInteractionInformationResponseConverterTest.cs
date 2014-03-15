//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Interaction.Transport;
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
    public sealed class EndpointInteractionInformationResponseConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new EndpointInteractionInformationResponseConverter();
            Assert.AreEqual(typeof(EndpointInteractionInformationResponseMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new EndpointInteractionInformationResponseConverter();
            Assert.AreEqual(typeof(EndpointInteractionInformationResponseData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new EndpointInteractionInformationResponseConverter();

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
            var translator = new EndpointInteractionInformationResponseConverter();

            var data = new EndpointInteractionInformationResponseData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                State = InteractionConnectionState.Desired.ToString(),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(EndpointInteractionInformationResponseMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreEqual(data.State, ((EndpointInteractionInformationResponseMessage)msg).State.ToString());
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new EndpointInteractionInformationResponseConverter();

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
            var translator = new EndpointInteractionInformationResponseConverter();

            var msg = new EndpointInteractionInformationResponseMessage(
                new EndpointId("a"), 
                new MessageId(), 
                InteractionConnectionState.Denied);
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(EndpointInteractionInformationResponseData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(msg.State.ToString(), ((EndpointInteractionInformationResponseData)data).State);
        }
    }
}
