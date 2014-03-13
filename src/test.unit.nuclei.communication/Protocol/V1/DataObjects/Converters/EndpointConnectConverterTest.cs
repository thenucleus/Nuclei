//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new EndpointConnectConverter();
            Assert.AreEqual(typeof(EndpointConnectMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new EndpointConnectConverter();
            Assert.AreEqual(typeof(EndpointConnectData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new EndpointConnectConverter();

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
            var translator = new EndpointConnectConverter();

            var data = new EndpointConnectData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                DiscoveryAddress = new Uri("http://localhost/discovery/invalid"),
                ProtocolVersion = new Version(1, 0),
                MessageAddress = new Uri("http://localhost/protocol/message/invalid"),
                DataAddress = new Uri("http://localhost/protocol/data/invalid"),
                Information = new ProtocolDescription(
                    new[]
                        {
                            new CommunicationSubject("b"), 
                        }),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(EndpointConnectMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
            Assert.AreSame(data.DiscoveryAddress, ((EndpointConnectMessage)msg).DiscoveryInformation.Address);
            Assert.AreSame(data.ProtocolVersion, ((EndpointConnectMessage)msg).ProtocolInformation.Version);
            Assert.AreSame(data.MessageAddress, ((EndpointConnectMessage)msg).ProtocolInformation.MessageAddress);
            Assert.AreSame(data.DataAddress, ((EndpointConnectMessage)msg).ProtocolInformation.DataAddress);
            Assert.AreSame(data.Information, ((EndpointConnectMessage)msg).Information);

        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new EndpointConnectConverter();

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
            var translator = new EndpointConnectConverter();

            var msg = new EndpointConnectMessage(
                new EndpointId("a"), 
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")), 
                new ProtocolInformation(
                    new Version(1, 0), 
                    new Uri("http://localhost/protocol/invalid")), 
                new ProtocolDescription(
                    new[]
                        {
                            new CommunicationSubject("b"), 
                        }));
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(EndpointConnectData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreSame(msg.DiscoveryInformation.Address, ((EndpointConnectData)data).DiscoveryAddress);
            Assert.AreSame(msg.ProtocolInformation.Version, ((EndpointConnectData)data).ProtocolVersion);
            Assert.AreSame(msg.ProtocolInformation.MessageAddress, ((EndpointConnectData)data).MessageAddress);
            Assert.AreSame(msg.ProtocolInformation.DataAddress, ((EndpointConnectData)data).DataAddress);
            Assert.AreSame(msg.Information, ((EndpointConnectData)data).Information);
        }
    }
}
